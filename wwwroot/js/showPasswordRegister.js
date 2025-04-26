document.addEventListener('DOMContentLoaded', function () {
    const eyeIcon = document.querySelector('.bi-eye, .bi-eye-slash');
    const passwordInput1 = document.querySelector('#inputPassword1');
    const passwordInput2 = document.querySelector('#inputPassword2');

    eyeIcon.addEventListener('click', function () {
        if (eyeIcon.classList.contains('bi-eye')) {
            eyeIcon.classList.remove('bi-eye');
            eyeIcon.classList.add('bi-eye-slash');
            passwordInput1.setAttribute('type', 'text');
            passwordInput2.setAttribute('type', 'text');
        } else {
            eyeIcon.classList.remove('bi-eye-slash');
            eyeIcon.classList.add('bi-eye');
            passwordInput1.setAttribute('type', 'password');
            passwordInput2.setAttribute('type', 'password');
        }
    });
});
