    $(document).ready(function() {
        $('#loginForm').on('submit', function (event) {
            event.preventDefault(); // Prevenir la recarga de la página
            var loginUrl = $(this).data('login-url'); // Obtener la URL desde el atributo data-login-url
            var formData = $(this).serialize(); // Obtener los datos del formulario

            $.ajax({
                type: 'POST',
                url: loginUrl,
                data: formData,
                success: function (response) {
                    if (response.success) {
                        // Redirigir si el login fue exitoso
                        window.location.href = response.redirectUrl;
                    } else {
                        // Mostrar el mensaje de error
                        $('#errorMessage').text(response.errorMessage).removeClass('d-none');
                    }
                },
                error: function (xhr, status, error) {
                    $('#errorMessage').text('An unexpected error occurred. Please try again.').removeClass('d-none');
                }
            });
        });
    });
