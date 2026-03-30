(function () {
  const apiBase = resolveApiBase();

  const loginForm = document.getElementById("loginForm");
  const registerForm = document.getElementById("registerForm");
  const message = document.getElementById("authMessage");

  if (!loginForm || !registerForm || !message) {
    return;
  }

  if (localStorage.getItem("qm_token")) {
    window.location.href = "home.html";
  }

  loginForm.addEventListener("submit", async function (event) {
    event.preventDefault();

    const username = document.getElementById("loginUsername").value.trim();
    const password = document.getElementById("loginPassword").value;

    if (!username || !password) {
      setMessage("Username and password are required.", true);
      return;
    }

    setMessage("Signing in...");

    try {
      const response = await fetch(
        `${apiBase}/api/auth/login?username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`,
        {
          method: "POST",
          headers: {
            Accept: "application/json"
          }
        }
      );

      if (!response.ok) {
        throw new Error(await extractErrorMessage(response));
      }

      const token = await extractToken(response);
      if (!token) {
        throw new Error("Login succeeded but token was empty.");
      }

      localStorage.setItem("qm_token", token);
      localStorage.setItem("qm_username", username);
      window.location.href = "home.html";
    } catch (error) {
      setMessage(normalizeFetchError(error, apiBase), true);
    }
  });

  registerForm.addEventListener("submit", async function (event) {
    event.preventDefault();

    const username = document.getElementById("registerUsername").value.trim();
    const password = document.getElementById("registerPassword").value;

    if (!username || !password) {
      setMessage("Username and password are required.", true);
      return;
    }

    setMessage("Creating account...");

    try {
      const response = await fetch(
        `${apiBase}/api/auth/register?username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`,
        {
          method: "POST",
          headers: {
            Accept: "application/json"
          }
        }
      );

      if (!response.ok) {
        throw new Error(await extractErrorMessage(response));
      }

      setMessage("Account created. You can now login.");
      registerForm.reset();
    } catch (error) {
      setMessage(normalizeFetchError(error, apiBase), true);
    }
  });

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

  function setMessage(text, isError) {
    message.textContent = text;
    message.style.color = isError ? "#8d1f1f" : "#173b34";
  }

  async function extractToken(response) {
    const contentType = response.headers.get("content-type") || "";

    if (contentType.includes("application/json")) {
      const payload = await response.json();
      if (typeof payload === "string") {
        return payload;
      }
      if (payload && typeof payload.token === "string") {
        return payload.token;
      }
      return "";
    }

    return (await response.text()).trim();
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
