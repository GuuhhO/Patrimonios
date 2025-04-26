document.addEventListener('DOMContentLoaded', function () {
    const eyeIcon = document.querySelector('.bi-eye, .bi-eye-slash');
    const passwordInput = document.querySelector('#inputPassword');

    eyeIcon.addEventListener('click', function () {
        if (eyeIcon.classList.contains('bi-eye')) {
            eyeIcon.classList.remove('bi-eye');
            eyeIcon.classList.add('bi-eye-slash');
            passwordInput.setAttribute('type', 'text');
        } else {
            eyeIcon.classList.remove('bi-eye-slash');
            eyeIcon.classList.add('bi-eye');
            passwordInput.setAttribute('type', 'password');
        }
    });
});
