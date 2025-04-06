function navigateToViewCards() {
    const cardSetId = new URLSearchParams(window.location.search).get("cardSetId");
    if (cardSetId) {
        window.location.href = `/view-cards?cardSetId=${cardSetId}`;
    } else {
        alert("Invalid Card Set ID!");
    }
}

const token = localStorage.getItem("jwt");

if (!token) {
    alert("You are not logged in!");
    window.location.href = "/login";
}
const urlParams = new URLSearchParams(window.location.search);
const cardSetId = urlParams.get("cardSetId");

if (!cardSetId) {
    alert("Invalid Card Set ID!");
    window.location.href = "dashboard";
}

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

        const cardSetDetails = await cardSetResponse.json();
        cardSetAccessType = cardSetDetails.accessType;

        // Show or hide the add card button based on access type
        const addCardButton = document.querySelector(".add-card-btn");
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

function updateCardDisplay() {
    const card = cards[currentCardIndex];
    document.getElementById("cardFront").textContent = card.term;
    document.getElementById("cardBack").textContent = card.definition;
    document.getElementById("cardCounter").textContent = `${currentCardIndex + 1}/${cards.length}`;

    // Show or hide the edit button based on access type
    const editButton = document.querySelector(".edit-button");
    if (cardSetAccessType === "OWNER") {
        editButton.style.display = "block";
    } else {
        editButton.style.display = "none";
    }
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

function openEditModal() {
    const card = cards[currentCardIndex];
    document.getElementById("editFrontText").value = card.term;
    document.getElementById("editBackText").value = card.definition;
    document.getElementById("editCardModal").style.display = "flex";
}

function closeEditModal() {
    document.getElementById("editCardModal").style.display = "none";
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

async function updateCard() {
    const frontText = document.getElementById("editFrontText").value.trim();
    const backText = document.getElementById("editBackText").value.trim();

    if (!frontText || !backText) {
        alert("Both front and back text are required!");
        return;
    }

    const CardRecord = {
        term: frontText,
        definition: backText
    };

    try {
        const response = await fetch(`/cards/${cards[currentCardIndex].id}`, {
            method: "PUT",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(CardRecord)
        });

        if (!response.ok) {
            throw new Error("Failed to update card");
        }

        // Update the card in the local array
        cards[currentCardIndex].term = frontText;
        cards[currentCardIndex].definition = backText;

        closeEditModal();
        updateCardDisplay();
    } catch (error) {
        console.error("Error updating card:", error);
        alert("Failed to update card. Please try again.");
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