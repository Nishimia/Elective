function search(inputId, rowClass, fieldsToSearchFilter) {
	let input = document.getElementById(inputId);
	let searchedValue = input.value.toUpperCase();
	let rows = Array.prototype.slice.call(document.querySelectorAll(rowClass));
	selectRows(rows, searchedValue, fieldsToSearchFilter);
}

function selectRows(rows, searchedValue, fieldsToSearchFilter) {
	for (let i = 0; i < rows.length; i++) {
		let fieldsToCheck = Array.prototype.slice.call(rows[i].querySelectorAll(fieldsToSearchFilter));
		if (fieldsToCheck.some(field => field.textContent.toUpperCase().includes(searchedValue))) {
			rows[i].style.display = "";
		} else {
			rows[i].style.display = "none";
		}
	}
}

function selectRowsUsingDropdown(rows, searchedValues, fieldsToSearchFilter) {
	for (let j = 0; j < rows.length; j++) {
		for (let i = 0; i < searchedValues.length; i++) {
			let fieldsToCheck = Array.prototype.slice.call(rows[j].querySelectorAll(fieldsToSearchFilter));
			if (fieldsToCheck.some(field => field.value.toUpperCase().includes(searchedValues[i].value.toUpperCase()))) {
				rows[j].style.display = "";
				break;
			} else {
				rows[j].style.display = "none";
			}
		}
	}
}

function searchDropDown(inputId, rowClass, fieldsToSearchFilter) {
	let input = document.getElementById(inputId);
	let searchedValues = input.querySelectorAll('option');
	let rows = Array.prototype.slice.call(document.querySelectorAll(rowClass));
	if (searchedValues.length == 0) {
		rows.forEach(row => row.style.display = "");
	} else {
		selectRowsUsingDropdown(rows, searchedValues, fieldsToSearchFilter);
	}
}