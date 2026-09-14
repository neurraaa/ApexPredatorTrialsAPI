const API_BASE = 'http://localhost:5050/api';
const ROUND_ORDER = ['Quarter-Final', 'Semi-Final', 'Final'];

function getToken() { return localStorage.getItem('token'); }
function getUser() {
  const stored = localStorage.getItem('user');
  return stored ? JSON.parse(stored) : null;
}
function isAdmin() {
  const user = getUser();
  return !!user && user.role === 'Admin';
}

function canManageEvent(event) {
  const user = getUser();
  return !!user && (user.role === 'Admin' || user.id === event.organizerId);
}

function playerLink(id, name) {
  if (!id) return 'TBD';
  return `<a href="player.html?id=${id}">${name}</a>`;
}

function matchupLabel(slot) {
  const hunter = playerLink(slot.hunterPlayerId, slot.hunterPlayerName);
  const human = playerLink(slot.humanPlayerId, slot.humanPlayerName);
  return `${hunter} (Hunter) vs ${human} (Human)`;
}
function setSession(token, user) {
  localStorage.setItem('token', token);
  localStorage.setItem('user', JSON.stringify(user));
}
function clearSession() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
}

const SESSION_TIMEOUT_MS = 5 * 60 * 1000;
const LAST_ACTIVITY_KEY = 'lastActivity';

function isTokenExpired(token) {
  try {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64 + '='.repeat((4 - (base64.length % 4)) % 4);
    const payload = JSON.parse(atob(padded));
    return !payload.exp || Date.now() >= payload.exp * 1000;
  } catch {
    return true;
  }
}

function markActivity() {
  if (getUser()) localStorage.setItem(LAST_ACTIVITY_KEY, String(Date.now()));
}

function expireSession() {
  if (!getUser()) return;
  clearSession();
  localStorage.removeItem(LAST_ACTIVITY_KEY);

  const header = document.getElementById('site-header');
  if (header) {
    header.innerHTML = `
      <nav>
        <a href="index.html">Home</a> | <a href="auth.html">Login / Register</a>
      </nav>
      <p>You were logged out due to inactivity. Please log in again.</p>
      <hr>
    `;
  }
}

function checkSessionTimeout() {
  if (!getUser()) return;

  const token = getToken();
  if (token && isTokenExpired(token)) {
    expireSession();
    return;
  }

  const lastActivity = Number(localStorage.getItem(LAST_ACTIVITY_KEY)) || Date.now();
  if (Date.now() - lastActivity >= SESSION_TIMEOUT_MS) {
    expireSession();
  }
}

checkSessionTimeout();
markActivity();
['mousemove', 'mousedown', 'keydown', 'scroll', 'touchstart'].forEach((evt) =>
  document.addEventListener(evt, markActivity, { passive: true })
);
setInterval(checkSessionTimeout, 15000);

async function apiFetch(path, options = {}) {
  const headers = { 'Content-Type': 'application/json', ...(options.headers || {}) };
  const token = getToken();
  if (token) headers['Authorization'] = `Bearer ${token}`;
 
  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });
  if (response.status === 204) return null;

  const text = await response.text();
  let data = null;
  try { data = text ? JSON.parse(text) : null; } catch { data = null; }

  if (!response.ok) {
    const message = data && data.error ? data.error
      : typeof data === 'string' ? data
      : data ? JSON.stringify(data)
      : text || response.statusText;
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