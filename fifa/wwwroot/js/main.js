// For kicking non-authorized people
if(!localStorage.getItem('token')) {
    window.location.href = '/Main/Login';
}
// Chat
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

connection.start().catch(err => console.error(err.toString()));

connection.on('Send', (message) => {
    appendLine(message);
});
try {
    document.getElementById('frm-send-message').addEventListener('submit', event => {
        let message = document.getElementById('message').value;
        document.getElementById('message').value = '';
        if (['Osman bad', 'Barca bad'].indexOf(message) > -1) {
            $('#error').html('Do not write this');
            setTimeout(() => {
                $('#error').html('');
            }, 2000);
            return;
        }

        connection.invoke('Send', message);
        event.preventDefault();
    });
} catch {
    console.log('Nu esli tak to eto catch');
}

function appendLine(line, color) {
    let li = document.createElement('li');
    li.innerText = line;
    document.getElementById('messages').appendChild(li);
}
// PlaySeason
connection.on('PlaySeason', (message) => {
    alert(message);
});

function playSeason(season) {
    connection.invoke('PlaySeason', season);
}
// Test
function testCall(season) {
    connection.invoke('Test', season);
}
connection.on('Test', (message) => {
    alert(message);
});