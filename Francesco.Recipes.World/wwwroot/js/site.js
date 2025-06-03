htmx.on('htmx:afterSwap', (event) => {
    if (event.target.id === 'instructions-container') {
        console.log('Instructions reloaded.');
    }
    if (event.target.id === 'ingredients-container') {
        console.log('Ingredients reloaded.');
    }
});

let recipeId;
function setRecipeId(id) {
    recipeId = id;
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
            if (element) {
                element.remove();
            }
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
            if (element) {
                element.remove();
            }
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
            console.log('Fetched units:', units);
            unitSelect.innerHTML = '';
            units.forEach(unit => {
                const option = new Option(unit.name, unit.id);
                unitSelect.add(option);
            });
        } else {
            console.error('Failed to fetch units');
            unitSelect.innerHTML = '<option value="">Fehler beim Laden</option>';
        }
    } catch (error) {
        console.error('Error fetching units:', error);
        unitSelect.innerHTML = '<option value="">Fehler beim Laden</option>';
    }
}


        async function addSelectedIngredientsToShoppingList() {
            var form = document.getElementById('ingredient-form');
            var formData = new FormData(form);
            var selectedIngredientIds = formData.getAll('ingredientIds');

            var response = await fetch('/ShoppingList/CreateOrAddIngredients', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ recipeId: '@Model.Id', ingredientIds: selectedIngredientIds })
            });

            if (response.ok) {
                var result = await response.json();
                localStorage.setItem('shoppingListId', result.shoppingListId);
                alert('Shopping list updated.');
            } else {
                alert('Failed to update shopping list.');
            }
}