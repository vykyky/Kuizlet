function navigateToViewCards() {
    const cardSetId = new URLSearchParams(window.location.search).get("cardSetId");
    if (cardSetId) {
        window.location.href = `../pages/view-cards.html?cardSetId=${cardSetId}`;
    } else {
        alert("Invalid Card Set ID!");
    }
}

const token = localStorage.getItem("jwt");

if (!token) {
    alert("You are not logged in!");
    window.location.href = "../pages/login.html";
}
const urlParams = new URLSearchParams(window.location.search);
const cardSetId = urlParams.get("cardSetId");

if (!cardSetId) {
    alert("Invalid Card Set ID!");
    window.location.href = "../pages/dashboard.html";
}
let cardSetDetails = null;

let cards = [];
let currentCardIndex = 0;
let cardSetAccessType = null;

async function fetchCards() {
    try {
        // Fetch the card set details
        const cardSetResponse = await fetch(`/cardsets/${cardSetId}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!cardSetResponse.ok) {
            throw new Error("Failed to fetch card set details");
        }

        cardSetDetails = await cardSetResponse.json();
        cardSetAccessType = cardSetDetails.accessType;
        document.getElementById("cardSetName").textContent = cardSetDetails.name;

        // Show or hide the add card button based on access type
        const addCardButton = document.querySelector(".dropdown");
        if (cardSetAccessType === "OWNER") {
            addCardButton.style.display = "block";
        } else {
            addCardButton.style.display = "none";
        }

        // Fetch the cards
        const cardsResponse = await fetch(`/cards/allCards/${cardSetId}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!cardsResponse.ok) {
            throw new Error(`Failed to fetch cards: ${cardsResponse.status}`);
        }

        cards = await cardsResponse.json();
        if (cards.length > 0) {
            updateCardDisplay();
            document.querySelector(".card-container").style.display = "block";
            document.querySelector(".navigation").style.display = "flex";
            document.getElementById("noCardsMessage").style.display = "none";
        } else {
            document.querySelector(".card-container").style.display = "none";
            document.querySelector(".navigation").style.display = "none";
            document.getElementById("noCardsMessage").style.display = "block";
        }
    } catch (error) {
        console.error("Error fetching cards:", error);
        alert("Something went wrong. Try again later.");
    }
}

document.addEventListener('keydown', function(event) {
   
    switch(event.key) {
        case 'ArrowLeft':
            
            showPreviousCard();
            break;
        case 'ArrowRight':
          
            showNextCard();
            break;
        case ' ':
           
            event.preventDefault(); 
            flipCard();
            break;
    }
});

function openEditCardSetModal() {
    if (cardSetDetails) {
        document.getElementById("editCardSetName").value = cardSetDetails.name;
        document.getElementById("editIsPublic").checked = cardSetDetails.isPublic || false;
        document.getElementById("editCardSetModal").style.display = "flex";
    }
}

function closeEditCardSetModal() {
    document.getElementById("editCardSetModal").style.display = "none";
}

async function updateCardSet() {
    const name = document.getElementById("editCardSetName").value.trim();
    const isPublic = document.getElementById("editIsPublic").checked;

    if (!name) {
        alert("All fields are required!");
        return;
    }

    const updatedCardSet = {
        Id: cardSetId,
        Name: name,
        IsPublic: isPublic
    };

    try {
        const response = await fetch(`/cardsets/${cardSetId}`, {
            method: "PUT",
            headers: {
                "Authorization": 'Bearer ' + token,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updatedCardSet)
        });

        if (!response.ok) {
            throw new Error("Failed to update card set");
        }

        const updatedDetails = await response.json();
        cardSetDetails = updatedDetails;
        document.getElementById("cardSetName").textContent = cardSetDetails.name;
        closeEditCardSetModal();
    } catch (error) {
        console.error("Error updating card set:", error);
        alert("Failed to update card set. Please try again.");
    }
}

function updateCardDisplay() {
    const card = cards[currentCardIndex];
    document.getElementById("cardFront").textContent = card.term;
    document.getElementById("cardBack").textContent = card.definition;
    document.getElementById("cardCounter").textContent = `${currentCardIndex + 1}/${cards.length}`;
}

function flipCard() {
    const cardElement = document.querySelector(".card");
    cardElement.classList.toggle("flipped");
}

function showNextCard() {
    if (currentCardIndex < cards.length - 1) {
        currentCardIndex++;
        updateCardDisplay();
    }
}

function showPreviousCard() {
    if (currentCardIndex > 0) {
        currentCardIndex--;
        updateCardDisplay();
    }
}

function openAddCardModal() {
    document.getElementById("addCardModal").style.display = "flex";
}

function closeAddCardModal() {
    document.getElementById("addCardModal").style.display = "none";
}

async function addCard() {
    const frontText = document.getElementById("addFrontText").value.trim();
    const backText = document.getElementById("addBackText").value.trim();

    if (!frontText || !backText) {
        alert("Both front and back text are required!");
        return;
    }

    const newCard = {
        term: frontText,
        definition: backText,
        cardSetId: cardSetId
    };

    try {
        const response = await fetch(`/cards`, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(newCard)
        });

        if (!response.ok) {
            throw new Error("Failed to add card");
        }

        const addedCard = await response.json();
        cards.push(addedCard);
        closeAddCardModal();
        updateCardDisplay();
    } catch (error) {
        console.error("Error adding card:", error);
        alert("Failed to add card. Please try again.");
    }
}


async function deleteCardSet() {
    const confirmDelete = confirm("Are you sure you want to delete this card set? This action cannot be undone.");
    if (!confirmDelete) return;

    try {
        const response = await fetch(`/cardsets/${cardSetId}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error("Failed to delete card set");
        }

        alert("Card set deleted successfully!");
        window.location.href = "../pages/dashboard.html"; // Redirect to dashboard
    } catch (error) {
        console.error("Error deleting card set:", error);
        alert("Failed to delete card set. Please try again.");
    }
}


fetchCards();