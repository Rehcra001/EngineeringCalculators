export function alertUser() {
    alert('The button was selected!');
}

export function addHandlers() {
    const btn = document.getElementById("btn");
    btn.addEventListener("click", getDirectoryHandle);
}

export async function getDirHandle() {
    const dirHandle = await window.showDirectoryPicker();

    console.log(dirHandle);
    return dirHandle;
}

