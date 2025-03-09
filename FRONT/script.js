document.getElementById("loginForm").addEventListener("submit", function(event) {
    event.preventDefault(); // Impede o envio do formulário

    let email = document.getElementById("email").value;
    let password = document.getElementById("password").value;
    let captcha = document.getElementById("captcha").checked;

    if (!captcha) {
        alert("Por favor, marque a opção Captcha.");
        return;
    }

    alert("Login realizado com sucesso!");
});
