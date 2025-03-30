document.getElementById("registerForm").addEventListener("submit", async function(event) {
    event.preventDefault();

    const fullName = document.getElementById('fullName').value;
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    const response = await fetch('/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ fullName, username, password })
    });

    const message = document.getElementById('message');
    if (response.status === 201) {
        message.style.color = 'green';
        message.innerText = "Registration successful! You can now log in.";
        setTimeout(() => window.location.href = '/login', 2000);
    } else if (response.status === 409) {
        message.style.color = 'red';
        message.innerText = "User already exists!";
    } else {
        message.style.color = 'red';
        message.innerText = "Registration failed!";
    }
});