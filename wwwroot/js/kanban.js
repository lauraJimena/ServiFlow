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



let tareaArrastrada = null;

function arrastrarTarea(event) {
    const tarea = event.target.closest(".tarea-card");
    const tareaId = tarea.dataset.id;

    tareaArrastrada = tarea;

    event.dataTransfer.setData("text/plain", tareaId);
    tarea.classList.add("dragging");

    const clon = tarea.cloneNode(true);
    clon.style.position = "absolute";
    clon.style.top = "-9999px";
    clon.style.left = "-9999px";
    clon.style.opacity = "1";
    clon.style.transform = "scale(1.03)";
    clon.style.boxShadow = "0 0 20px rgba(181, 23, 255, 0.35)";
    clon.style.background = "#d9d9df";

    document.body.appendChild(clon);
    event.dataTransfer.setDragImage(clon, 20, 20);

    setTimeout(() => {
        document.body.removeChild(clon);
    }, 0);
}

function permitirDrop(event) {
    event.preventDefault();
    event.currentTarget.classList.add("drag-over");
}

async function soltarTarea(event, nuevoEstado) {
    event.preventDefault();

    const columnaDestino = event.currentTarget;
    columnaDestino.classList.remove("drag-over");

    const tareaId = event.dataTransfer.getData("text/plain");
    if (!tareaId || !tareaArrastrada) return;

    const columnaOrigen = tareaArrastrada.parentElement;

    tareaArrastrada.classList.remove("dragging");
    tareaArrastrada.classList.add("drop-in");

    const mensajeVacioDestino = columnaDestino.querySelector(".mensaje-vacio");
    if (mensajeVacioDestino) {
        mensajeVacioDestino.style.display = "none";
    }

    columnaDestino.appendChild(tareaArrastrada);

    actualizarMensajesVacios();

    const token = document.querySelector('#formMoverDrag input[name="__RequestVerificationToken"]')?.value;

    const formData = new FormData();
    formData.append("tareaId", tareaId);
    formData.append("nuevoEstado", nuevoEstado);
    formData.append("emprendimientoId", document.querySelector('#formMoverDrag input[name="emprendimientoId"]').value);
    formData.append("__RequestVerificationToken", token);

    try {
        const response = await fetch('/Kanban/Mover', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            columnaOrigen.appendChild(tareaArrastrada);
            actualizarMensajesVacios();
            alert("No se pudo mover la tarea.");
        }
    } catch (error) {
        columnaOrigen.appendChild(tareaArrastrada);
        actualizarMensajesVacios();
        alert("Ocurrió un error al mover la tarea.");
    }

    tareaArrastrada.classList.remove("drop-in");
    tareaArrastrada = null;
}

function actualizarMensajesVacios() {
    document.querySelectorAll(".columna-kanban").forEach(columna => {
        const tareas = columna.querySelectorAll(".tarea-card");
        const mensaje = columna.querySelector(".mensaje-vacio");

        if (!mensaje) return;

        if (tareas.length === 0) {
            mensaje.style.display = "block";
        } else {
            mensaje.style.display = "none";
        }
    });
}

document.addEventListener("dragend", function (event) {
    const tarea = event.target.closest(".tarea-card");
    if (tarea) {
        tarea.classList.remove("dragging");
    }

    document.querySelectorAll(".columna-kanban").forEach(columna => {
        columna.classList.remove("drag-over");
    });
});

document.querySelectorAll(".columna-kanban").forEach(columna => {
    columna.addEventListener("dragleave", function (event) {
        if (!this.contains(event.relatedTarget)) {
            this.classList.remove("drag-over");
        }
    });
});