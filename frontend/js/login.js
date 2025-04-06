document.getElementById("loginForm").addEventListener("submit", async function(event) {
    event.preventDefault(); // Prevent default form submission

    const Languageogin = document.getElementById('login').value;
    const Password = document.getElementById('password').value;

    const response = await fetch('/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Login, Password })
    });
username
    const message = document.getElementById('message');
    if (response.ok) {
        const token = await response.text();
        localStorage.setItem('jwt', token);
        localStorage.setItem('login', Login);
        message.style.color = 'green';
        message.innerText = "Login successful! Redirecting...";
        setTimeout(() => window.location.href = '../pages/dashboard.html', 2000);
    } else {
        message.style.color = 'red';
        message.innerText = "Invalid credentials!";
    }
});