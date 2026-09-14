const output = document.getElementById('output');
function showOutput(data) { output.textContent = JSON.stringify(data, null, 2); }

document.getElementById('register-form').addEventListener('submit', async (e) => {
  e.preventDefault();
  try {
    const result = await apiFetch('/auth/register', {
      method: 'POST',
      body: JSON.stringify({
        username: document.getElementById('register-username').value,
        password: document.getElementById('register-password').value
      })
    });
    setSession(result.token, result.user);
    showOutput(result);
    window.location.href = 'index.html';
  } catch (err) {
    showOutput({ error: err.message });
  }
});

document.getElementById('login-form').addEventListener('submit', async (e) => {
  e.preventDefault();
  try {
    const result = await apiFetch('/auth/login', {
      method: 'POST',
      body: JSON.stringify({
        username: document.getElementById('login-username').value,
        password: document.getElementById('login-password').value
      })
    });
    setSession(result.token, result.user);
    showOutput(result);
    window.location.href = 'index.html';
  } catch (err) {
    showOutput({ error: err.message });
  }
});

renderHeader();
