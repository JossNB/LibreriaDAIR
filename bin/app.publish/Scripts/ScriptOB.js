let pasoActual = 0; //Indice de cada paso del onboarding
const steps = document.querySelectorAll('.onboarding-step');//Cada paso del onboarding
const indicatorsContainer = document.getElementById('indicators');
const onboardingContainer = document.getElementById('onboardingContainer');

const mainPageURL = "/Home/Index"; //Direccion a la que se redirige al cliente después de completar el onboarding

// Generar indicadores dinámicos
steps.forEach((_, index) => {
    const indicator = document.createElement('div');
    indicator.classList.add('indicator');
    if (index === 0) indicator.classList.add('active');
    indicatorsContainer.appendChild(indicator);
});

// Se verifica si ya se completó el onboarding y se guarda en el localstorage
if (!localStorage.getItem('onboardingCompletado')) {
    window.onload = function () {
        onboardingContainer.style.display = 'flex'; //Si no te ha completado se muestra el onboarding
        showStep(pasoActual);
    };
} else {
    window.location.href = mainPageURL; // Si ya fue completado se redirigire a la página principal
}

//Se muestra paso por paso, bloqueando los demás
function showStep(step) {
    steps.forEach((el, index) => {
        el.style.display = index === step ? 'block' : 'none';
    });
    updateIndicators(step);
}

// Actualizar indicadores
function updateIndicators(step) {
    const indicators = document.querySelectorAll('.indicator');
    indicators.forEach((el, index) => {
        el.classList.toggle('active', index === step);
    });
}

// Avanzar al siguiente paso
function nextStep() {
    if (pasoActual < steps.length - 1) {
        pasoActual++;
        showStep(pasoActual);
    } else {
        completeOnboarding(); //Cuando se llega la ultimo paso se completa el onboarding
    }
}

//Se guarda el onboarding como completado en el local storage
function completeOnboarding() {
    localStorage.setItem('onboardingCompletado', 'true');
    window.location.href = mainPageURL; //Se redirige a la página principal
}

//El cliente puede saltarse el onboarding y se guarda como completado
function skipOnboarding() {
    localStorage.setItem('onboardingCompletado', 'true');
    window.location.href = mainPageURL; //Se redirige a la página principal
}
