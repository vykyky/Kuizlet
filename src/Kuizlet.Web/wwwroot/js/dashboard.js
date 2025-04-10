let currentCardSetId = null;

async function createCardSet() {
    const token = localStorage.getItem('jwt');
    if (!token) {
        alert("You are not logged in!");
        return;
    }
    const setName = document.getElementById('setName').value.trim();
    const isPublic = document.getElementById('isPublic').checked;
    if (!setName) {
        alert("Card set name cannot be empty!");
        return;
    }
    const newCardSet = {
        Name: setName,
        IsPublic: isPublic,
    };
    try {
        const response = await fetch('/cardsets', {    
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newCardSet)
        });
        if (!response.ok) {
            throw new Error("Failed to create card set");
        }
        closeModal();
        renderLibraryCardSets();
    } catch (error) {
        console.error(error);
        alert("Error creating card set");
    }
}

async function fetchAllCardSets() {      
    const token = localStorage.getItem('jwt');
    if (!token) {
        window.location.href = '../pages/login.html';
        return;
    }
    const response = await fetch('/cardsets/all', {        //получение всех карточек. вызывается в
        headers: { 'Authorization': 'Bearer ' + token }
    });
    if (response.ok) {
        return await response.json();
    } else {
        console.error('Error fetching all card sets');
        return [];
    }
}

async function fetchCardSets() {
    const token = localStorage.getItem('jwt');
    if (!token) {
        window.location.href = '../pages/login.html';
        return;
    }
    const response = await fetch('/cardsets/all', {     //получение всех карточек. по коду если честно то же самое что и прошлое
        headers: { 'Authorization': 'Bearer ' + token }
    });
    if (response.ok) {
        return await response.json();
    } else {
        console.error('Error fetching card sets');
        return [];
    }
}

async function fetchPendingRequests() {
    const token = localStorage.getItem('jwt');
    if (!token) {
        window.location.href = '../pages/login.html';
        return [];
    }
    const login = localStorage.getItem('login');
    if (!login) {
        console.error('Login not found');
        return [];
    }
    const allCardSets = await fetchCardSets();
    const userCardSets = allCardSets.filter(set => set.creatorLogin === login);
    const allRequests = [];
    for (const cardSet of userCardSets) {
        const response = await fetch(`/request/${cardSet.id}/requests`, {
            headers: { 'Authorization': 'Bearer ' + token }
        });
        if (response.ok) {
            const requests = await response.json();
            allRequests.push(...requests);
        } else {
            console.error(`Error fetching requests for card set ${cardSet.id}`);
        }
    }
    return allRequests;
}

async function handleRequest(requestId, cardSetId, action) {
    const token = localStorage.getItem('jwt');
    if (!token) {
        alert("You are not logged in!");
        return;
    }
    try {
        const response = await fetch(`/request/${cardSetId}/requests/${requestId}?approve=${action === 'accept'}`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error("Failed to handle request");
        }
        const result = await response.json();
        if (result.message) {
            alert(result.message);
        } else if (result.error) {
            alert(result.error);
        }
        renderNotifications();
    } catch (error) {
        console.error(error);
        alert("Error handling request");
    }
}

function logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('login');
    window.location.href = '../pages/login.html';
}

function navigate(page) {
    const buttons = document.querySelectorAll('.sidebar button');
    buttons.forEach(button => button.classList.remove('active'));
    const activeButton = document.querySelector(`.sidebar button[onclick="navigate('${page}')"]`);
    activeButton.classList.add('active');
    switch (page) {
        case 'home':
            
            renderAllCardSets();
            break;
        case 'library':
            
            renderLibraryCardSets();
            break;
        case 'notifications':
           
            renderNotifications();
            break;
        default:
            console.error('Invalid page:', page);
    }
}

async function openCardSet(cardSetId) {
    const token = localStorage.getItem('jwt');
    if (!token) {
        alert("You are not logged in!");
        return;
    }
    try {
        const response = await fetch(`/cardsets/${cardSetId}`, {
            headers: { 'Authorization': 'Bearer ' + token }
        });
        if (!response.ok) {
            throw new Error("You do not have access to this card set");
        }
        window.location.href = `../pages/cards.html?cardSetId=${cardSetId}`;
    } catch (error) {
        console.error(error);
        alert(error.message);
    }
}

function openModal() {
    document.getElementById('cardSetModal').style.display = 'flex';
}

function closeModal() {
    document.getElementById('cardSetModal').style.display = 'none';
}

function closeUpdateModal() {
    document.getElementById('updateCardSetModal').style.display = 'none';
}

async function updateCardSet() {
    const token = localStorage.getItem('jwt');
    if (!token) {
        alert("You are not logged in!");
        return;
    }
    const setName = document.getElementById('updateSetName').value.trim();
    const isPublic = document.getElementById('updateIsPublic').checked;
    if (!setName) {
        alert("Card set name cannot be empty!");
        return;
    }
    const updatedCardSet = {
        name: setName,
        isPublic: isPublic,
    };
    try {
        const response = await fetch(`/cardsets/${currentCardSetId}`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedCardSet)
        });
        if (!response.ok) {
            throw new Error("Failed to update card set");
        }
        closeUpdateModal();
        renderLibraryCardSets();
    } catch (error) {
        console.error(error);
        alert("Error updating card set");
    }
}

async function renderAllCardSets() {
    const login = localStorage.getItem('login');
    const container = document.querySelector('.main-content .container');
    container.innerHTML = `
    <div class="header">
        <span class="login-display">👋 Welcome, ${login || 'User'}!</span>
        <button class="logout-btn" onclick="logout()">Logout</button>
    </div>
    <h2>🏠 Home</h2>
    <div class="search-container">
        <input type="text" id="searchBar" class="search-box" placeholder="🔍 Search all card sets..." onkeyup="filterAllCardSets()">
    </div>
    <div id="all-cardsets" class="card-grid">Loading...</div>
`;

    const allCardSets = await fetchAllCardSets();  // ВОЗМОЖНО УБРАТЬ
    const allCardSetsContainer = document.getElementById('all-cardsets');
    allCardSetsContainer.innerHTML = '';

    const filteredCardSets = allCardSets.filter(set => set.accessType !== 'PRIVATE');

    const uniqueCardSets = {};
    filteredCardSets.forEach(set => {
        if (!uniqueCardSets[set.id] || set.accessType === 'OWNER') {
            uniqueCardSets[set.id] = set;
        }
    });

    Object.values(uniqueCardSets).forEach(set => {
        let buttonHTML = '';
        let accessTypeHTML = '';

        if (set.accessType === 'OWNER' || set.accessType === 'ACCESSIBLE') {
            accessTypeHTML = `<div class="access-type">${set.accessType}</div>`;
        } else if (set.accessType === 'PUBLIC') {
            buttonHTML = `<button class="btn" onclick="requestAccess('${set.id}')">Send Request</button>`;
        } else if (set.accessType === 'PENDING') {
            buttonHTML = `<button class="btn" disabled>PENDING</button>`;
        } else if (set.accessType === 'DECLINED') {
            buttonHTML = `
            <button class="btn" disabled>DECLINED</button>
            <button class="btn small-btn" onclick="requestAccess('${set.id}')">Send Request Again</button>
        `;
        }

        const cardSetHTML = `
        <div class="cardset" ${set.accessType === 'OWNER' || set.accessType === 'ACCESSIBLE' ? `onclick="openCardSet('${set.id}')"` : ''}>
            <h3>${set.name}</h3>
            ${accessTypeHTML}
            ${buttonHTML}
        </div>
    `;
        allCardSetsContainer.innerHTML += cardSetHTML;
    });
}

async function renderLibraryCardSets() {
    const container = document.querySelector('.main-content .container');
    container.innerHTML = `
        <div class="header">
            <button class="btn" onclick="openModal()">➕ Add Card Set</button>
        </div>
        <h2>📚 Your Library</h2>
        <div class="search-container">
            <input type="text" id="searchBar" class="search-box" placeholder="🔍 Search your card sets..." onkeyup="filterCardSets()">
        </div>
        <div id="cardsets" class="card-grid">Loading...</div>
    `;

    const allCardSets = await fetchCardSets();
    const login = localStorage.getItem('login');

    const userCardSets = allCardSets.filter(set =>
        set.creatorLogin === login && (set.accessType === 'OWNER' || set.accessType === 'PRIVATE')
    );

    renderCardSets(userCardSets);
}

function renderCardSets(cardSets) {
    const cardsetsContainer = document.getElementById('cardsets');
    if (!cardsetsContainer) {
        console.error("Cardsets container not found!");
        return;
    }

    if (cardSets.length === 0) {
        cardsetsContainer.innerHTML = `<p>No card sets found.</p>`;
        return;
    }

    cardsetsContainer.innerHTML = cardSets.map(set => `
        <div class="cardset" onclick="openCardSet('${set.id}')">
            <h3>${set.name}</h3>
            <div class="access-type">${set.accessType}</div>
        </div>
    `).join('');
}

async function renderNotifications() {
    const container = document.querySelector('.main-content .container');
    container.innerHTML = `
        <div class="header">
            <h2>🔔 Notifications</h2>
        </div>
        <div id="notifications" class="card-grid">Loading...</div>
    `;
    const pendingRequests = await fetchPendingRequests();
    const notificationsContainer = document.getElementById('notifications');
    notificationsContainer.innerHTML = '';
    if (pendingRequests.length === 0) {
        notificationsContainer.innerHTML = `<p>You have no new notifications.</p>`;
        return;
    }
    pendingRequests.forEach(request => {
        const notificationHTML = `
            <div class="notification-item">
                <p>User <strong>${request.requesterLogin}</strong> requested access to <strong>${request.cardSetName}</strong>.</p>
                <div class="notification-actions">
                    <button class="accept" onclick="handleRequest('${request.id}', '${request.cardSetId}', 'accept')">Accept</button>
                    <button class="reject" onclick="handleRequest('${request.id}', '${request.cardSetId}', 'reject')">Reject</button>
                </div>
            </div>
        `;
        notificationsContainer.innerHTML += notificationHTML;
    });
}

async function requestAccess(cardSetId) {
    const token = localStorage.getItem('jwt');
    if (!token) {
        alert("You are not logged in!");
        return;
    }
    try {
        const response = await fetch(`/request/${cardSetId}/request-access`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error("Failed to request access");
        }
        const result = await response.json();
        alert(result.message);
        const cardSetElement = document.querySelector(`.cardset[onclick*="${cardSetId}"]`);
        if (cardSetElement) {
            const button = cardSetElement.querySelector('.btn');
            if (button) {
                button.textContent = "Request Sent";
                button.disabled = true;
            }
        }
    } catch (error) {
        console.error(error);
        alert("Error requesting access");
    }
}

function filterAllCardSets() {
    const searchText = document.getElementById('searchBar').value.toLowerCase();
    const cardsets = document.querySelectorAll('#all-cardsets .cardset');
    cardsets.forEach(set => {
        const name = set.querySelector('h3').textContent.toLowerCase();
        if (name.includes(searchText)) {
            set.style.display = 'block';
        } else {
            set.style.display = 'none';
        }
    });
}

function filterCardSets() {
    const searchText = document.getElementById('searchBar').value.toLowerCase();
    const cardsets = document.querySelectorAll('#cardsets .cardset');
    cardsets.forEach(set => {
        const name = set.querySelector('h3').textContent.toLowerCase();
        if (name.includes(searchText)) {
            set.style.display = 'block';
        } else {
            set.style.display = 'none';
        }
    });
}

navigate('home');