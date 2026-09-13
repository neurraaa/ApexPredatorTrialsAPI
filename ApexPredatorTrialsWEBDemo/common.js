const API_BASE = 'http://localhost:5050/api';

function getToken() { return localStorage.getItem('token'); }
function getUser() {
  const stored = localStorage.getItem('user');
  return stored ? JSON.parse(stored) : null;
}
function setSession(token, user) {
  localStorage.setItem('token', token);
  localStorage.setItem('user', JSON.stringify(user));
}
function clearSession() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
}

async function apiFetch(path, options = {}) {
  const headers = { 'Content-Type': 'application/json', ...(options.headers || {}) };
  const token = getToken();
  if (token) headers['Authorization'] = `Bearer ${token}`;
 
  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });
  const data = response.status === 204 ? null : await response.json().catch(() => null);
 
  if (!response.ok) {
    const message = data && data.error ? data.error
      : typeof data === 'string' ? data
      : data ? JSON.stringify(data)
      : response.statusText;
    throw new Error(message);
  }
  return data;
}

function renderHeader() {
  const header = document.getElementById('site-header');
  if (!header) return;
 
  const user = getUser();
  header.innerHTML = `
    <nav>
      <a href="index.html">Home</a> |
      ${user
        ? `Logged in as <span id="header-username">${user.username}</span> (${user.role}) - <button id="logout-btn">Logout</button>`
        : `<a href="auth.html">Login / Register</a>`}
    </nav>
    <hr>
  `;

  const logoutBtn = document.getElementById('logout-btn');
  if (logoutBtn) {
    logoutBtn.addEventListener('click', () => {
      clearSession();
      window.location.href = 'index.html';
    });
  }

  if (user) {
    apiFetch('/players/me')
      .then((myPlayer) => {
        const span = document.getElementById('header-username');
        if (span) span.innerHTML = `<a href="player.html?id=${myPlayer.id}">${user.username}</a>`;
      })
      .catch(() => { /* no linked Player for this account */ });
  }
}