var isAscending = true;
function sortElements(rowClass, fieldToSortClass) {
	let rows = Array.prototype.slice.call(document.querySelectorAll(rowClass));
	let sortedRows = rows.sort((a, b) => {
		let aValue = a.querySelector(fieldToSortClass).textContent;
		let bValue = b.querySelector(fieldToSortClass).textContent;
		if (aValue > bValue) {
			return 1;
		} else if (aValue < bValue) {
			return -1;
		} else {
			return 0
		}
	});
	if (isAscending) {
		isAscending = false;
		return sortedRows;
	}
	else {
		isAscending = true;
		return sortedRows.reverse();
	}
}

function removeAllChildren(node) {
	while (node.firstChild) {
		node.removeChild(node.firstChild);
	}
}

function sortTable(tableId, rowClass, fieldToSortClass, isAscending) {
	let table = document.querySelector(tableId);
	let sorted_rows = sortElements(rowClass, fieldToSortClass, isAscending);
	removeAllChildren(table);
	sorted_rows.forEach((row) => table.appendChild(row));
}