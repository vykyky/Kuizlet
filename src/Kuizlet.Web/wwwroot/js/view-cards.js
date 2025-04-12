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
    let cards = [];
    let currentCardId = null;
    
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
            const cardList = document.getElementById("cardList");
            cardList.innerHTML = ""; // Очищаем список

            // Создаём кликабельные карточки
            cards.forEach(card => {
                const cardElement = document.createElement("div");
                cardElement.className = "card-item";
                cardElement.innerHTML = `
                    <h3>${card.term}</h3>
                    <p>${card.definition}</p>
                `;
    
                // Добавляем обработчик клика
                cardElement.addEventListener("click", () => {
                    openEditModal(card.id); // Открываем модальное окно с данными карточки
                });
    
                cardList.appendChild(cardElement);
            });
    
        } catch (error) {
            console.error("Error fetching cards:", error);
            alert("Something went wrong. Try again later.");
        }
    }

    function openEditModal(cardId) {
        currentCardId = cardId; // Сохраняем ID для использования в saveCardChanges
        const card = cards.find(c => c.id === cardId);
        
        if (!card) {
            console.error("Card not found");
            return;
        }
    
        document.getElementById("editFrontText").value = card.term;
        document.getElementById("editBackText").value = card.definition;
        document.getElementById("editCardModal").style.display = "flex";
    }
    
    function closeEditModal() {
        document.getElementById("editCardModal").style.display = "none";
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
            const response = await fetch(`/cards/${currentCardId}`, {
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
            const cardIndex = cards.findIndex(card => card.id === currentCardId);
            if (cardIndex !== -1) {
                cards[cardIndex] = {
                    ...cards[cardIndex],  // Сохраняем остальные поля карточки
                    term: frontText,
                    definition: backText
                };
            }
            fetchCards();
    
            closeEditModal();
        } catch (error) {
            console.error("Error updating card:", error);
            alert("Failed to update card. Please try again.");
        }
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
    function back(){
        window.location.href=`../pages/cards.html?cardSetId=${cardSetId}`;
    }
    fetchCards();