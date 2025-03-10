document.addEventListener('DOMContentLoaded', function() {

    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: '<',
            center: 'title',
            right: '>'
        },

        locale: 'pt-br',
        fixedWeekCount: false,
        firstDay: 0, // Começando no Domingo
        contentHeight: 'auto',
        events: [] // Adicione eventos se necessário

    });

    calendar.render();

});
