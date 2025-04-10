const token = localStorage.getItem("jwt");

    if (!token) {
        alert("You are not logged in!");
        window.location.href = "../pages/login.html";
    }

    const urlParams = new URLSearchParams(window.location.search);
    const cardSetId = urlParams.get("cardSetId");
    console.log(cardSetId);

    if (!cardSetId) {
        alert("Invalid Card Set ID!");
        window.location.href = "../pages/dashboard.html";
    }

    let cardSetDetails = null;

    async function fetchCards() {
        try {
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

            document.getElementById("cardSetName").textContent = cardSetDetails.name;
           

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

            const cards = await cardsResponse.json();
            const cardList = document.getElementById("cardList");
            cardList.innerHTML = cards.map(card => `
                <div class="card-item">
                    <h3>${card.term}</h3>
                    <p>${card.definition}</p>
                </div>
            `).join('');
        } catch (error) {
            console.error("Error fetching cards:", error);
            alert("Something went wrong. Try again later.");
        }
    }

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
            document.getElementById("cardSetName").textContent = updatedDetails.name;
           
            closeEditCardSetModal();
        } catch (error) {
            console.error("Error updating card set:", error);
            alert("Failed to update card set. Please try again.");
        }
    }

    function openAddCardModal() {
        if (cardSetDetails) {
            document.getElementById("termInput").placeholder = `Enter term`;
            document.getElementById("definitionInput").placeholder = `Enter definition`;
            document.getElementById("addCardModal").style.display = "flex";
        }
    }

    function closeAddCardModal() {
        document.getElementById("addCardModal").style.display = "none";
        clearInputFields();
    }

    function clearInputFields() {
        document.getElementById("termInput").value = "";
        document.getElementById("definitionInput").value = "";
    }

    async function addCard() {
        const term = document.getElementById("termInput").value.trim();
        const definition = document.getElementById("definitionInput").value.trim();

        if (!term || !definition) {
            alert("Both term and definition are required!");
            return;
        }

        const newCard = {
            term: term,
            definition: definition,
            cardSetId: cardSetId
        };

        try {
            const response = await fetch(`/cards/${cardSetId}`, {
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
            closeAddCardModal();
            clearInputFields();
            fetchCards();
        } catch (error) {
            console.error("Error adding card:", error);
            alert("Failed to add card. Please try again.");
        }
    }
    fetchCards();