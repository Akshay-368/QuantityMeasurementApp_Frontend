(function () {
  const token = localStorage.getItem("qm_token");
  if (!token) {
    window.location.href = "index.html";
    return;
  }

  const apiBase = resolveApiBase();

  const unitsByCategory = {
    Length: ["Feet", "Inch", "Yard", "Meter", "Centimeter"],
    Weight: ["Kilogram", "Gram", "Pound"],
    Volume: ["Litre", "Millilitre", "Gallon"],
    Temperature: ["Celsius", "Fahrenheit", "Kelvin"]
  };

  const operatorMap = {
    Convert: "convert",
    Add: "add",
    Subtract: "subtract",
    DivideByScalar: "divide-scalar",
    DivideByQuantity: "divide-quantity"
  };

  const categorySelect = document.getElementById("category");
  const operatorSelect = document.getElementById("operator");
  const unit1Select = document.getElementById("unit1");
  const unit2Select = document.getElementById("unit2");
  const targetUnitSelect = document.getElementById("targetUnit");

  const value1Input = document.getElementById("value1");
  const value2Input = document.getElementById("value2");
  const scalarInput = document.getElementById("scalar");

  const value2Wrap = document.getElementById("value2Wrap");
  const unit2Wrap = document.getElementById("unit2Wrap");
  const targetUnitWrap = document.getElementById("targetUnitWrap");
  const scalarWrap = document.getElementById("scalarWrap");

  const operationForm = document.getElementById("operationForm");
  const resultText = document.getElementById("resultText");

  const historyTableBody = document.getElementById("historyTableBody");
  const historyMessage = document.getElementById("historyMessage");
  const refreshHistoryBtn = document.getElementById("refreshHistoryBtn");
  const clearHistoryBtn = document.getElementById("clearHistoryBtn");
  const logoutBtn = document.getElementById("logoutBtn");

  bootstrap();

  function bootstrap() {
    fillSelect(categorySelect, Object.keys(unitsByCategory));
    fillSelect(operatorSelect, Object.keys(operatorMap));

    syncUnits();
    syncVisibleFields();

    categorySelect.addEventListener("change", function () {
      syncUnits();
      syncVisibleFields();
    });

    operatorSelect.addEventListener("change", syncVisibleFields);

    operationForm.addEventListener("submit", runOperation);
    refreshHistoryBtn.addEventListener("click", loadHistory);
    clearHistoryBtn.addEventListener("click", clearHistory);
    logoutBtn.addEventListener("click", logout);

    loadHistory();
  }

  function syncUnits() {
    const category = categorySelect.value;
    const units = unitsByCategory[category] || [];

    fillSelect(unit1Select, units);
    fillSelect(unit2Select, units);
    fillSelect(targetUnitSelect, units);
  }

  function syncVisibleFields() {
    const operator = operatorSelect.value;

    hide(value2Wrap, unit2Wrap, targetUnitWrap, scalarWrap);

    if (operator === "Convert") {
      show(targetUnitWrap);
      targetUnitSelect.required = true;
      value2Input.required = false;
      unit2Select.required = false;
      scalarInput.required = false;
      return;
    }

    if (operator === "Add" || operator === "Subtract") {
      show(value2Wrap, unit2Wrap, targetUnitWrap);
      targetUnitSelect.required = true;
      value2Input.required = true;
      unit2Select.required = true;
      scalarInput.required = false;
      return;
    }

    if (operator === "DivideByScalar") {
      show(scalarWrap);
      scalarInput.required = true;
      targetUnitSelect.required = false;
      value2Input.required = false;
      unit2Select.required = false;
      return;
    }

    if (operator === "DivideByQuantity") {
      show(value2Wrap, unit2Wrap);
      value2Input.required = true;
      unit2Select.required = true;
      targetUnitSelect.required = false;
      scalarInput.required = false;
    }
  }

  async function runOperation(event) {
    event.preventDefault();

    const operator = operatorSelect.value;
    const endpoint = operatorMap[operator];

    if (!endpoint) {
      setResult("Unknown operator selected.", true);
      return;
    }

    const body = {
      value1: toNumber(value1Input.value),
      unit1: unit1Select.value,
      value2: null,
      unit2: null,
      targetUnit: null,
      scalar: null
    };

    if (operator === "Convert") {
      body.targetUnit = targetUnitSelect.value;
    }

    if (operator === "Add" || operator === "Subtract") {
      body.value2 = toNumber(value2Input.value);
      body.unit2 = unit2Select.value;
      body.targetUnit = targetUnitSelect.value;
    }

    if (operator === "DivideByScalar") {
      body.scalar = toNumber(scalarInput.value);
    }

    if (operator === "DivideByQuantity") {
      body.value2 = toNumber(value2Input.value);
      body.unit2 = unit2Select.value;
    }

    setResult("Running operation...");

    try {
      const response = await secureFetch(`/api/quantity/${endpoint}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
      });

      if (!response.ok) {
        throw new Error(await extractErrorMessage(response));
      }

      const payload = await response.json();
      const prettyResult = formatResult(operator, payload);
      setResult(prettyResult);
      loadHistory();
    } catch (error) {
      setResult(normalizeFetchError(error, apiBase), true);
    }
  }

  async function loadHistory() {
    historyMessage.textContent = "Loading history...";
    historyMessage.style.color = "#173b34";

    try {
      const response = await secureFetch("/api/history", { method: "GET" });
      if (!response.ok) {
        throw new Error(await extractErrorMessage(response));
      }

      const history = await response.json();
      renderHistory(history);
      historyMessage.textContent = `Loaded ${history.length} row(s).`;
    } catch (error) {
      historyMessage.textContent = normalizeFetchError(error, apiBase);
      historyMessage.style.color = "#8d1f1f";
    }
  }

  async function clearHistory() {
    historyMessage.textContent = "Clearing history...";
    historyMessage.style.color = "#173b34";

    try {
      const response = await secureFetch("/api/history", { method: "DELETE" });
      if (!response.ok) {
        throw new Error(await extractErrorMessage(response));
      }

      renderHistory([]);
      historyMessage.textContent = "History cleared.";
    } catch (error) {
      historyMessage.textContent = normalizeFetchError(error, apiBase);
      historyMessage.style.color = "#8d1f1f";
    }
  }

  function resolveApiBase() {
    const fallback = "https://localhost:7051";
    const protocol = window.location.protocol;
    const origin = window.location.origin;

    if (!protocol || protocol === "file:") {
      return fallback;
    }

    if (origin && origin !== "null") {
      return origin;
    }

    return fallback;
  }

  function normalizeFetchError(error, resolvedApiBase) {
    const message = error && error.message ? error.message : "Request failed.";
    if (message === "Failed to fetch") {
      return `Failed to fetch. Ensure API is running at ${resolvedApiBase} and open the app from the API URL (not file path).`;
    }
    return message;
  }

  function renderHistory(rows) {
    historyTableBody.innerHTML = "";

    if (!Array.isArray(rows) || rows.length === 0) {
      const tr = document.createElement("tr");
      const td = document.createElement("td");
      td.colSpan = 4;
      td.textContent = "No history yet.";
      tr.appendChild(td);
      historyTableBody.appendChild(tr);
      return;
    }

    rows.forEach(function (row) {
      const tr = document.createElement("tr");

      const dateCell = document.createElement("td");
      dateCell.textContent = new Date(row.createdAt).toISOString();

      const opCell = document.createElement("td");
      opCell.textContent = row.operation;

      const inputCell = document.createElement("td");
      inputCell.textContent = formatInput(row);

      const outputCell = document.createElement("td");
      outputCell.textContent = formatOutput(row);

      tr.appendChild(dateCell);
      tr.appendChild(opCell);
      tr.appendChild(inputCell);
      tr.appendChild(outputCell);
      historyTableBody.appendChild(tr);
    });
  }

  function formatInput(row) {
    const left = `${row.value1} ${row.unit1}`;

    if (row.operation === "Convert") {
      return `${left} to ${row.targetUnit}`;
    }

    if (row.operation === "Add" || row.operation === "Subtract") {
      return `${left} and ${row.value2} ${row.unit2} => ${row.targetUnit}`;
    }

    if (row.operation === "DivideByScalar") {
      return `${left} / ${row.scalar}`;
    }

    if (row.operation === "DivideByQuantity") {
      return `${left} / ${row.value2} ${row.unit2}`;
    }

    return left;
  }

  function formatOutput(row) {
    if (!row.resultUnit) {
      return `${row.result}`;
    }
    return `${row.result} ${row.resultUnit}`;
  }

  function formatResult(operator, payload) {
    if (operator === "DivideByQuantity") {
      return `Result: ${payload}`;
    }

    return `Result: ${payload.result} ${payload.unit}`;
  }

  async function secureFetch(path, options) {
    const response = await fetch(`${apiBase}${path}`, {
      ...options,
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${token}`,
        ...(options && options.headers ? options.headers : {})
      }
    });

    if (response.status === 401) {
      logout();
      throw new Error("Session expired. Please login again.");
    }

    return response;
  }

  function logout() {
    localStorage.removeItem("qm_token");
    localStorage.removeItem("qm_username");
    window.location.href = "index.html";
  }

  function fillSelect(selectElement, values) {
    selectElement.innerHTML = "";
    values.forEach(function (value) {
      const option = document.createElement("option");
      option.value = value;
      option.textContent = value;
      selectElement.appendChild(option);
    });
  }

  function hide() {
    Array.prototype.forEach.call(arguments, function (element) {
      element.style.display = "none";
    });
  }

  function show() {
    Array.prototype.forEach.call(arguments, function (element) {
      element.style.display = "grid";
    });
  }

  function setResult(text, isError) {
    resultText.textContent = text;
    resultText.style.color = isError ? "#8d1f1f" : "#173b34";
  }

  function toNumber(value) {
    return Number(value);
  }

  async function extractErrorMessage(response) {
    const contentType = response.headers.get("content-type") || "";

    if (contentType.includes("application/problem+json") || contentType.includes("application/json")) {
      const payload = await response.json();
      return payload.detail || payload.title || JSON.stringify(payload);
    }

    return (await response.text()) || `Request failed with status ${response.status}.`;
  }
})();
