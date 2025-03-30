document.getElementById("loginForm").addEventListener("submit", async function(event) {
    event.preventDefault(); // Prevent default form submission

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    const response = await fetch('/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
    });

    const message = document.getElementById('message');
    if (response.ok) {
        const token = await response.text();
        localStorage.setItem('jwt', token);
        localStorage.setItem('username', username);
        message.style.color = 'green';
        message.innerText = "Login successful! Redirecting...";
        setTimeout(() => window.location.href = '/dashboard', 2000);
    } else {
        message.style.color = 'red';
        message.innerText = "Invalid credentials!";
    }
});