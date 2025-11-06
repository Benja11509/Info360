// Carga y muestra tareas desde localStorage y filtra por título
document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('buscador');
    const lista = document.getElementById('lista');
    const buscadorContainer = document.getElementById('buscadorContainer');
    const mostrarBuscar = document.getElementById('mostrarBuscar');
    const tareasContainer = document.getElementById('tareasContainer'); // 👈 agregado
  
  
    // Obtener tareas desde localStorage
    let tareas = [];
    try {
      tareas = JSON.parse(localStorage.getItem('tareas') || '[]');
    } catch (e) {
      tareas = [];
    }
  
  
    // Si no hay tareas, crear un ejemplo (no sobreescribe si ya hay datos)
    if (!tareas || tareas.length === 0) {
      tareas = [
        { titulo: 'Pasar a Alta', fecha: '23/10/25', descripcion: 'Pasar el figma' },
        { titulo: 'Enviar informe', fecha: '30/10/25', descripcion: 'Enviar por email' },
        { titulo: 'Reunión con equipo', fecha: '01/11/25', descripcion: 'Planificación' }
      ];
      localStorage.setItem('tareas', JSON.stringify(tareas));
    }
  
  
    // Renderizar lista SIEMPRE al cargar
    function renderList(items) {
      lista.innerHTML = '';
      items.forEach((t, idx) => {
        const div = document.createElement('div');
        div.className = 'tarea-card';
        div.innerHTML = `
          <span>📄 ${t.titulo}</span>
          <span>Finalizado</span>
          <div class="estado"></div>
        `;
        div.addEventListener('click', () => {
          localStorage.setItem('tareaSeleccionada', JSON.stringify(t));
          window.location.href = 'verInfoTarea.html';
        });
        lista.appendChild(div);
      });
    }
    renderList(tareas);
  
  
    // Mostrar buscador al tocar Buscar (solo en a.html)
    if (mostrarBuscar && buscadorContainer && !window.location.pathname.includes('Buscar.html')) {
      mostrarBuscar.addEventListener('click', (e) => {
        e.preventDefault();
  
  
        // 👇 agregado: oculta la tarea visible
        if (tareasContainer) tareasContainer.style.display = 'none';
  
  
        // 👇 muestra el buscador
        buscadorContainer.style.display = 'block';
        input.value = '';
        renderList(tareas);
        input.focus();
      });
    }
  
  
    // Filtrar tareas mientras se escribe
    if (input) {
      input.addEventListener('input', () => {
        const texto = input.value.trim().toLowerCase();
        if (texto === '') {
          renderList(tareas);
        } else {
          const filtradas = tareas.filter(t => t.titulo.toLowerCase().includes(texto));
          renderList(filtradas);
        }
      });
    }
  
  
    // Ocultar buscador si se hace clic fuera
    document.addEventListener('click', (e) => {
      if (buscadorContainer.style.display === 'block') {
        const isInside = buscadorContainer.contains(e.target) || mostrarBuscar.contains(e.target);
        if (!isInside) {
          buscadorContainer.style.display = 'none';
  
  
          // 👇 agregado: vuelve a mostrar las tareas al cerrar el buscador
          if (tareasContainer) tareasContainer.style.display = 'block';
        }
      }
    });
  });
  
  
  