import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from '../../services/auth';
import { HistoryService } from '../../services/history';
import { QuantityService } from '../../services/quantity';

type Category = 'Length' | 'Weight' | 'Volume' | 'Temperature';
type Operation = 'Convert' | 'Add' | 'Subtract' | 'DivideByScalar' | 'DivideByQuantity';

interface UnitOption {
  label: string;
  value: string;
}

interface QuantityResult {
  result: number;
  unit: string;
}

interface HistoryRow {
  id: string;
  operation: string;
  value1: number;
  unit1: string;
  value2?: number;
  unit2?: string;
  targetUnit?: string;
  scalar?: number;
  result: number;
  resultUnit: string;
  createdAt: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  readonly categories: Category[] = ['Length', 'Weight', 'Volume', 'Temperature'];
  readonly operators: Operation[] = ['Convert', 'Add', 'Subtract', 'DivideByScalar', 'DivideByQuantity'];

  readonly unitsByCategory: Record<Category, UnitOption[]> = {
    Length: [
      { label: 'Feet', value: 'feet' },
      { label: 'Inches', value: 'inch' },
      { label: 'Yard', value: 'yard' },
      { label: 'Meter', value: 'meter' },
      { label: 'Centimeter', value: 'centimeter' }
    ],
    Weight: [
      { label: 'Kilogram', value: 'kilogram' },
      { label: 'Gram', value: 'gram' },
      { label: 'Pound', value: 'pound' }
    ],
    Volume: [
      { label: 'Litre', value: 'litre' },
      { label: 'Millilitre', value: 'millilitre' },
      { label: 'Gallon', value: 'gallon' }
    ],
    Temperature: [
      { label: 'Kelvin', value: 'kelvin' },
      { label: 'Fahrenheit', value: 'fahrenheit' },
      { label: 'Celsius', value: 'celsius' }
    ]
  };

  readonly operatorMap: Record<Operation, string> = {
    Convert: 'convert',
    Add: 'add',
    Subtract: 'subtract',
    DivideByScalar: 'divide-scalar',
    DivideByQuantity: 'divide-quantity'
  };

  category: Category = 'Length';
  operator: Operation = 'Convert';
  unit1 = this.unitsByCategory.Length[0].value;
  unit2 = this.unitsByCategory.Length[0].value;
  targetUnit = this.unitsByCategory.Length[0].value;

  value1: number | null = null;
  value2: number | null = null;
  scalar: number | null = null;

  result = '';
  history: HistoryRow[] = [];
  message = '';
  isCalculating = false;

  constructor(
    private quantity: QuantityService,
    private historyService: HistoryService,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  get units(): UnitOption[] {
    return this.unitsByCategory[this.category];
  }

  get selectedUser(): string {
    if (typeof localStorage === 'undefined') {
      return 'User';
    }

    return localStorage.getItem('qm_username') ?? 'User';
  }

  onCategoryChange(): void {
    const first = this.units[0].value;
    this.unit1 = first;
    this.unit2 = first;
    this.targetUnit = first;
    this.result = '';
  }

  onOperatorChange(): void {
    this.result = '';
    this.message = '';
  }

  isValidNumber(value: number | null): boolean {
    return value !== null && Number.isFinite(value);
  }

  runOperation(): void {
    if (this.isCalculating) {
      return;
    }

    const validationMessage = this.validateForm();
    if (validationMessage) {
      this.result = '';
      this.message = validationMessage;
      return;
    }

    const endpoint = this.operatorMap[this.operator];
    const body = this.buildPayload();

    this.isCalculating = true;
    this.message = '';
    this.result = 'Calculating...';

    this.quantity
      .runOperation(endpoint, body)
      .pipe(
        finalize(() => {
          this.isCalculating = false;
        })
      )
      .subscribe({
      next: (res: unknown) => {
        if (this.operator === 'DivideByQuantity') {
          this.result = `Result: ${res}`;
        } else {
          const typed = res as QuantityResult;
          this.result = `Result: ${typed.result} ${typed.unit}`;
        }

        this.loadHistory();
      },
      error: (err) => {
        this.result = '';
        this.message = this.auth.extractErrorMessage(err);
      }
    });
  }

  loadHistory(): void {
    this.historyService.getHistory().subscribe({
      next: (res: unknown[]) => {
        this.history = res as HistoryRow[];
      },
      error: (err) => {
        this.message = this.auth.extractErrorMessage(err);
      }
    });
  }

  clearHistory(): void {
    this.historyService.clearHistory().subscribe({
      next: () => {
        this.history = [];
        this.message = 'History cleared.';
      },
      error: (err) => {
        this.message = this.auth.extractErrorMessage(err);
      }
    });
  }

  formatInput(row: HistoryRow): string {
    const first = `${row.value1} ${row.unit1}`;

    if (row.operation === 'Convert') {
      return `${first} -> ${row.targetUnit ?? ''}`;
    }

    if (row.operation === 'Add' || row.operation === 'Subtract' || row.operation === 'DivideByQuantity') {
      return `${first} and ${row.value2 ?? ''} ${row.unit2 ?? ''}`;
    }

    if (row.operation === 'DivideByScalar') {
      return `${first} by ${row.scalar ?? ''}`;
    }

    return first;
  }

  logout(): void {
    this.auth.logout();
  }

  private validateForm(): string {
    if (!this.isValidNumber(this.value1)) {
      return 'Value 1 must be a valid number.';
    }

    if ((this.operator === 'Add' || this.operator === 'Subtract' || this.operator === 'DivideByQuantity') && !this.isValidNumber(this.value2)) {
      return 'Value 2 must be a valid number for the selected operation.';
    }

    if (this.operator === 'DivideByScalar') {
      if (!this.isValidNumber(this.scalar)) {
        return 'Scalar must be a valid number.';
      }

      if (this.scalar === 0) {
        return 'Scalar cannot be zero.';
      }
    }

    if (this.operator === 'DivideByQuantity' && this.value2 === 0) {
      return 'Value 2 cannot be zero for divide-by-quantity.';
    }

    return '';
  }

  private buildPayload(): Record<string, unknown> {
    const payload: Record<string, unknown> = {
      value1: this.value1,
      unit1: this.unit1
    };

    if (this.operator === 'Convert') {
      payload['targetUnit'] = this.targetUnit;
    }

    if (this.operator === 'Add' || this.operator === 'Subtract') {
      payload['value2'] = this.value2;
      payload['unit2'] = this.unit2;
      payload['targetUnit'] = this.targetUnit;
    }

    if (this.operator === 'DivideByScalar') {
      payload['scalar'] = this.scalar;
    }

    if (this.operator === 'DivideByQuantity') {
      payload['value2'] = this.value2;
      payload['unit2'] = this.unit2;
    }

    return payload;
  }
}
