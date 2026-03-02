(function (window, document) {   
    const selectedIngredients = new Set();
    const recipeIngredients = window.recipeIngredients || {};
    let activeRecipeId = Object.keys(recipeIngredients)[0];
    let recipeId;

    htmx && htmx.on('htmx:afterSwap', (event) => {
        if (event.target.id === 'instructions-container') {
            console.log('Instructions reloaded.');
        }
        if (event.target.id === 'ingredients-container') {
            console.log('Ingredients reloaded.');
        }
    });

    function setRecipeId(id) {
        recipeId = id;
    }


    async function updateRecipeCount() {
        try {
            const response = await fetch('/ShoppingList/RecipeCount');
            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
            const data = await response.json();
            document.getElementById('recipeCount').textContent = data.count;
        } catch (err) {
            console.error("Fehler beim Aktualisieren der Rezeptanzahl:", err);
        }
    }

    const countUpdateInterval = setInterval(updateRecipeCount, 5000);

    document.addEventListener('shopping-list-changed', updateRecipeCount);

    window.addEventListener('beforeunload', function () {
        clearInterval(countUpdateInterval);
    });

    document.addEventListener('DOMContentLoaded', function () {
        document.getElementById('carouselLeft')?.addEventListener('click', function () {
            document.getElementById('recipeCarousel').scrollBy({ left: -200, behavior: 'smooth' });
        });
        document.getElementById('carouselRight')?.addEventListener('click', function () {
            document.getElementById('recipeCarousel').scrollBy({ left: 200, behavior: 'smooth' });
        });

        if (activeRecipeId) setActiveCard(activeRecipeId);

        document.querySelectorAll('.recipe-card').forEach(card => {
            card.addEventListener('click', function () {
                setActiveCard(this.dataset.recipeId);
            });
        });
    });

    function renderIngredientList(recipeId) {
        const list = document.getElementById('ingredientList');
        list.innerHTML = '';
        const ingredients = recipeIngredients[recipeId] || [];
        if (ingredients.length === 0) {
            list.innerHTML = '<li class="list-group-item text-muted">Keine Zutaten vorhanden.</li>';
            return;
        }
        ingredients.forEach(ingredient => {
            const isSelected = selectedIngredients.has(ingredient.id);
            const li = document.createElement('li');
            li.className = 'list-group-item d-flex justify-content-between align-items-center';
            if (isSelected) li.classList.add('selected-ingredient');
            li.innerHTML = `
                <span>
                <strong>${ingredient.name}</strong>
                <span class="text-secondary ms-2">${ingredient.amount} ${ingredient.unit}</span>
                </span>
                <button class="btn btn-sm btn-outline-danger" title="Zutat entfernen" onclick="Francesco.toggleIngredientSelection('${ingredient.id}', this); event.stopPropagation();">
                <i class="bi bi-dash"></i>
                </button>
            `;
            list.appendChild(li);
        });
        updateRemoveSelectedButton();
    }

    function setActiveCard(recipeId) {
        document.querySelectorAll('.recipe-card').forEach(card => {
            card.classList.toggle('active-recipe-card', card.dataset.recipeId === recipeId);
        });
        activeRecipeId = recipeId;
        renderIngredientList(recipeId);
    }

    function toggleIngredientSelection(ingredientId, button) {
        const listItem = button.closest('li');
        if (selectedIngredients.has(ingredientId)) {
            selectedIngredients.delete(ingredientId);
            listItem.classList.remove('selected-ingredient');
        } else {
            selectedIngredients.add(ingredientId);
            listItem.classList.add('selected-ingredient');
        }
        updateRemoveSelectedButton();
    }

    function updateRemoveSelectedButton() {
        document.getElementById('removeSelectedButton').style.display =
            selectedIngredients.size > 0 ? 'inline-block' : 'none';
    }

    async function removeSelectedIngredients() {
        if (selectedIngredients.size === 0) return;

        const shoppingListIds = [];
        selectedIngredients.forEach(ingredientId => {
            for (const recipeId in recipeIngredients) {
                const ingredients = recipeIngredients[recipeId];
                const ingredient = ingredients.find(ing => ing.id === ingredientId);
                if (ingredient && ingredient.shoppingListId &&
                    ingredient.shoppingListId !== "00000000-0000-0000-0000-000000000000") {
                    shoppingListIds.push(ingredient.shoppingListId);
                }
            }
        });

        if (shoppingListIds.length === 0) {
            console.warn("Keine gültigen Shopping List IDs gefunden");
            return;
        }

        try {
            const response = await fetch('/ShoppingList/RemoveIngredients', {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value,
                    'Cache-Control': 'no-cache',
                    'Pragma': 'no-cache'
                },
                body: JSON.stringify(shoppingListIds)
            });

            if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

            const result = await response.json();

            if (result.success) {
                window.location.reload();
            } else {
                console.error("Fehler beim Entfernen der Zutaten:", result.error);
                alert("Beim Entfernen der Zutaten ist ein Fehler aufgetreten.");
            }
        } catch (err) {
            console.error("Fehler beim Entfernen der Zutaten:", err);
            alert("Beim Entfernen der Zutaten ist ein Fehler aufgetreten. Bitte versuchen Sie es später erneut.");
        }
    }

    async function removeRecipe(recipeId) {
        let confirmed = false;

        if (window.Swal) {
            const result = await Swal.fire({
                title: 'Rezept entfernen?',
                text: 'Möchten Sie dieses Rezept wirklich aus der Einkaufsliste entfernen?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Ja, entfernen',
                cancelButtonText: 'Abbrechen',
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6'
            });
            confirmed = result.isConfirmed;
        } else {
            confirmed = confirm('Möchten Sie dieses Rezept wirklich aus der Einkaufsliste entfernen?');
        }

        if (!confirmed) return;

        try {
            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            if (!tokenElement) {
                console.error('Anti-forgery token not found');
                alert('Fehler: Anti-Forgery Token nicht gefunden');
                return;
            }

            const response = await fetch(`/ShoppingList/RemoveRecipeFromList/${recipeId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': tokenElement.value
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const result = await response.json();

            if (result.success) {
                if (window.Swal) {
                    await Swal.fire({
                        title: 'Erfolgreich!',
                        text: 'Das Rezept wurde entfernt.',
                        icon: 'success',
                        timer: 1500,
                        showConfirmButton: false
                    });
                } else {
                    alert('Das Rezept wurde erfolgreich entfernt.');
                }
                window.location.reload();
            } else {
                const errorMsg = result.error || 'Unbekannter Fehler';
                console.error('Fehler beim Entfernen:', errorMsg);
                if (window.Swal) {
                    await Swal.fire({
                        title: 'Fehler',
                        text: errorMsg,
                        icon: 'error'
                    });
                } else {
                    alert('Fehler: ' + errorMsg);
                }
            }
        } catch (err) {
            console.error('Fehler beim Entfernen des Rezepts:', err);
            if (window.Swal) {
                await Swal.fire({
                    title: 'Fehler',
                    text: 'Beim Löschen ist ein Fehler aufgetreten.',
                    icon: 'error'
                });
            } else {
                alert('Beim Löschen ist ein Fehler aufgetreten.');
            }
        }
    }


    async function moveInstructionUp(instructionId, recipeIdParam) {
        const idToUse = recipeIdParam || recipeId;
        if (!idToUse) {
            alert('Recipe ID is not set. Please select a recipe first.');
            return;
        }

        try {
            const response = await fetch(`/Recipe/${idToUse}/Instruction/${instructionId}/move-up`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.ok) {
                htmx.ajax('GET', `/Recipe/${idToUse}/Instructions`, '#instructions-container');
            } else {
                const error = await response.json();
                alert(error.Error || 'Failed to move instruction up.');
            }
        } catch (error) {
            console.error('Error moving instruction up:', error);
        }
    }

    async function moveInstructionDown(instructionId, recipeIdParam) {
        const idToUse = recipeIdParam || recipeId;
        if (!idToUse) {
            alert('Recipe ID is not set. Please select a recipe first.');
            return;
        }

        try {
            const response = await fetch(`/Recipe/${idToUse}/Instruction/${instructionId}/move-down`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.ok) {
                htmx.ajax('GET', `/Recipe/${idToUse}/Instructions`, '#instructions-container');
            } else {
                const error = await response.json();
                alert(error.Error || 'Failed to move instruction down.');
            }
        } catch (error) {
            console.error('Error moving instruction down:', error);
        }
    }

    async function removeInstruction(instructionId, recipeIdParam) {
        const idToUse = recipeIdParam || recipeId;
        if (!idToUse) {
            alert('Recipe ID is not set. Please select a recipe first.');
            return;
        }
        if (!confirm('Möchtest du diesen Schritt wirklich löschen?')) return;

        try {
            const response = await fetch(`/${idToUse}/RemoveInstruction/${instructionId}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.ok) {
                const element = document.getElementById(`instruction-${instructionId}`);
                if (element) element.remove();
            } else {
                const error = await response.json();
                alert(error.Error || 'Fehler beim Löschen der Anweisung.');
            }
        } catch (error) {
            console.error('Fehler beim Löschen:', error);
        }
    }

    function addInstruction() {
        const container = document.getElementById('instructions-container');
        const index = document.querySelectorAll('.instruction-item').length;

        const newInstructionHtml = `
            <div class="instruction-item" id="instruction-new-${index}">
                <div class="instruction-controls">
                    <input type="file" name="InstructionViewModel.Instructions[${index}].MediaFile" class="instruction-file" />
                    <textarea name="InstructionViewModel.Instructions[${index}].Description" placeholder="Beschreibung" class="form-control"></textarea>
                    <button type="button" class="btn-delete" onclick="document.getElementById('instruction-new-${index}').remove()">🗑️</button>
                </div>
                <div class="instruction-actions">
                    <button type="button" class="btn-move-up" disabled>⬆️</button>
                    <button type="button" class="btn-move-down" disabled>⬇️</button>
                </div>
            </div>
        `;
        container.insertAdjacentHTML('beforeend', newInstructionHtml);
    }

    async function removeIngredient(ingredientId) {
        if (!confirm('Möchtest du diese Zutat wirklich löschen?')) return;

        try {
            const response = await fetch(`/${recipeId}/RemoveIngredient/${ingredientId}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.ok) {
                const element = document.getElementById(`ingredient-${ingredientId}`);
                if (element) element.remove();
            } else {
                const error = await response.json();
                alert(error.Error || 'Fehler beim Löschen der Zutat.');
            }
        } catch (error) {
            console.error('Fehler beim Löschen:', error);
        }
    }

    async function addIngredient() {
        const container = document.getElementById('ingredients-container');
        const index = document.querySelectorAll('.ingredient-item').length;

        const newIngredientHtml = `
            <div class="ingredient-item" id="ingredient-new-${index}">
                <div class="ingredient-controls">
                    <input type="text" name="IngredientViewModel.Ingredients[${index}].Name" placeholder="Name" class="form-control" />
                    <input type="number" name="IngredientViewModel.Ingredients[${index}].RecipeIngredients[0].Quantity" placeholder="Menge" class="form-control" />
                    <select name="IngredientViewModel.Ingredients[${index}].RecipeIngredients[0].Unit.Id" class="form-control unit-select">
                        <option value="">Lade Einheiten...</option>
                    </select>
                    <button type="button" class="btn-delete" onclick="document.getElementById('ingredient-new-${index}').remove()">🗑️</button>
                </div>
            </div>
        `;
        container.insertAdjacentHTML('beforeend', newIngredientHtml);

        const addedElement = document.getElementById(`ingredient-new-${index}`);
        const unitSelect = addedElement.querySelector('.unit-select');

        try {
            const response = await fetch('/Unit/GetAllUnits');
            if (response.ok) {
                const units = await response.json();
                unitSelect.innerHTML = '';
                units.forEach(unit => {
                    const option = new Option(unit.name, unit.id);
                    unitSelect.add(option);
                });
            } else {
                unitSelect.innerHTML = '<option value="">Fehler beim Laden</option>';
            }
        } catch (error) {
            unitSelect.innerHTML = '<option value="">Fehler beim Laden</option>';
        }
    }

    window.Francesco = {
        setRecipeId,
        moveInstructionUp,
        moveInstructionDown,
        removeInstruction,
        addInstruction,
        removeIngredient,
        addIngredient,
        toggleIngredientSelection,
        removeSelectedIngredients,
        removeRecipe,
    };

})(window, document);
console.log("Francesco-Objekt initialisiert:", window.Francesco);