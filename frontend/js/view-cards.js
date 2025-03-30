const token = localStorage.getItem("jwt");

    if (!token) {
        alert("You are not logged in!");
        window.location.href = "/login";
    }

    const urlParams = new URLSearchParams(window.location.search);
    const cardSetId = urlParams.get("cardSetId");
    console.log(cardSetId);

    if (!cardSetId) {
        alert("Invalid Card Set ID!");
        window.location.href = "dashboard";
    }

    let cardSetDetails = null;

    function populateLanguageDropdowns() {
        const languages = [
            "English", "Spanish", "French", "German", "Russian", "Chinese", "Arabic", "Italian",
            "Portuguese", "Japanese", "Korean", "Hindi", "Turkish", "Dutch", "Swedish",
            "Polish", "Greek", "Romanian", "Indonesian", "Vietnamese", "Thai", "Bengali",
            "Persian", "Urdu", "Hebrew", "Uzbek"
        ];

        const firstLangSelect = document.getElementById('editFirstLanguage');
        const secondLangSelect = document.getElementById('editSecondLanguage');

        firstLangSelect.innerHTML = '';
        secondLangSelect.innerHTML = '';

        languages.forEach(lang => {
            const option = document.createElement('option');
            option.value = lang;
            option.textContent = lang;
            firstLangSelect.appendChild(option.cloneNode(true));
            secondLangSelect.appendChild(option.cloneNode(true));
        });
    }

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
            document.getElementById("cardSetLanguages").textContent = `First Language: ${cardSetDetails.firstLanguage}, Second Language: ${cardSetDetails.secondLanguage}`;

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
            document.getElementById("editFirstLanguage").value = cardSetDetails.firstLanguage;
            document.getElementById("editSecondLanguage").value = cardSetDetails.secondLanguage;
            document.getElementById("editIsPublic").checked = cardSetDetails.isPublic || false;
            document.getElementById("editCardSetModal").style.display = "flex";
        }
    }

    function closeEditCardSetModal() {
        document.getElementById("editCardSetModal").style.display = "none";
    }

    async function updateCardSet() {
        const name = document.getElementById("editCardSetName").value.trim();
        const firstLanguage = document.getElementById("editFirstLanguage").value.trim();
        const secondLanguage = document.getElementById("editSecondLanguage").value.trim();
        const isPublic = document.getElementById("editIsPublic").checked;

        if (!name || !firstLanguage || !secondLanguage) {
            alert("All fields are required!");
            return;
        }

        const updatedCardSet = {
            name: name,
            firstLanguage: firstLanguage,
            secondLanguage: secondLanguage,
            isPublic: isPublic
        };

        try {
            const response = await fetch(`/cardsets/${cardSetId}`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${token}`,
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
            document.getElementById("cardSetLanguages").textContent = `First Language: ${updatedDetails.firstLanguage}, Second Language: ${updatedDetails.secondLanguage}`;
            closeEditCardSetModal();
        } catch (error) {
            console.error("Error updating card set:", error);
            alert("Failed to update card set. Please try again.");
        }
    }

    function openAddCardModal() {
        if (cardSetDetails) {
            document.getElementById("termInput").placeholder = `Enter term in ${cardSetDetails.firstLanguage}`;
            document.getElementById("definitionInput").placeholder = `Enter definition in ${cardSetDetails.secondLanguage}`;
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

    // Initialize
    populateLanguageDropdowns();
    fetchCards();