function abrirPopupDesdeElemento(boton) {
    const card = boton.closest(".tarea-card");

    const id = card.dataset.id;
    const titulo = card.dataset.titulo;
    const descripcion = card.dataset.descripcion;

    document.getElementById("popupTitulo").innerText = titulo;
    document.getElementById("popupDescripcion").innerText =
        descripcion && descripcion.trim() !== "" ? descripcion : "Sin descripción";

    document.getElementById("phId").value = id;
    document.getElementById("epId").value = id;
    document.getElementById("tId").value = id;
    document.getElementById("deleteId").value = id;

    document.getElementById("popupTarea").style.display = "flex";
}

function cerrarPopup() {
    document.getElementById("popupTarea").style.display = "none";
}

window.addEventListener("click", function (event) {
    const popup = document.getElementById("popupTarea");

    if (event.target === popup) {
        cerrarPopup();
    }
});

function arrastrarTarea(event) {
    const tarea = event.target.closest(".tarea-card");
    const tareaId = tarea.dataset.id;

    event.dataTransfer.setData("text/plain", tareaId);
}

function permitirDrop(event) {
    event.preventDefault();
}

function soltarTarea(event, nuevoEstado) {
    event.preventDefault();

    const tareaId = event.dataTransfer.getData("text/plain");

    if (!tareaId) return;

    document.getElementById("dragTareaId").value = tareaId;
    document.getElementById("dragNuevoEstado").value = nuevoEstado;
    document.getElementById("formMoverDrag").submit();
}