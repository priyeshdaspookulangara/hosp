const loginForm = document.getElementById('login-form');
const loginSection = document.getElementById('login');
const dashboardSection = document.getElementById('dashboard');
const dashboardContent = document.getElementById('dashboard-content');

loginForm.addEventListener('submit', (e) => {
    e.preventDefault();
    // In a real application, you would send the login credentials to the backend for authentication.
    // For this example, we'll just hide the login form and show the dashboard.
    loginSection.classList.add('hidden');
    dashboardSection.classList.remove('hidden');
    dashboardContent.innerHTML = '<p>Welcome to the Radiology Information System!</p>';
});
