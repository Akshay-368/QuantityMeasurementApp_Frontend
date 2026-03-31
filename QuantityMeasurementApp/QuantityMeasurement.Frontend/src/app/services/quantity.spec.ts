import { TestBed } from '@angular/core/testing';

import { Quantity } from './quantity';

describe('Quantity', () => {
  let service: Quantity;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Quantity);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
