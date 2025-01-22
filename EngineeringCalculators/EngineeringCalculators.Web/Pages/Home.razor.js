export function alertUser() {
    alert('The button was selected!');
}

export function addHandlers() {
    const btn = document.getElementById("btn");
    btn.addEventListener("click", getDirectoryHandle);
}

export function getDirHandle() {
    const dirHandle = window.showDirectoryPicker();
    console.log(dirHandle);
    return JSON.stringify(dirHandle);
}

