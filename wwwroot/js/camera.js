var video;
var captureBtn;
var recaptureBtn;
var saveBtn;
var btnGroup;
var capturedImg;


const constraints = {
    video: true
};

const openCamera = async function () {
    $("#camera-model").modal({
        show: true,
        keyboard: false,
        backdrop: 'static'
    });

    // initialize components
    video = document.getElementById('video');
    captureBtn = document.getElementById('captureBtn');
    recaptureBtn = document.getElementById('recaptureBtn');
    saveBtn = document.getElementById('saveBtn');
    btnGroup = document.getElementById('btnGroup');
    capturedImg = document.getElementById('capturedImg');

    try {
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        video.srcObject = stream;
    } catch (err) {
        console.error('Error accessing webcam:', err);
    }

    return false;
}

const capturePhoto = () => {
    var canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);

    // Convert the canvas to a data URL
    var dataURL = canvas.toDataURL('image/png');

    // Display the captured photo
    capturedImg.src = dataURL;
    capturedImg.style.display = "block";

    // Hide the video and show recapture button
    video.style.display = "none";
    captureBtn.style.display = "none";
    btnGroup.style.display = "block";
    return false;
};

const reCapture = () => {
    // Hide the captured image and show the video feed
    capturedImg.style.display = "none";
    video.style.display = "inline-block";

    // Show capture button and hide recapture button
    captureBtn.style.display = "block";
    btnGroup.style.display = "none";
    return false;
};

const savePicture = () => {
    // Save Image and Create Preview
    document.getElementById('cameraPhoto').value = capturedImg.src;
    showImagePreview(0, 'photo', capturedImg.src);

    // Stop camera stream
    var videoElement = document.querySelector('video');
    videoElement.srcObject.getTracks().forEach(function (track) {
        track.stop();
    });

    // Close Modal Popup
    $("#camera-model .modal-body").html('');
    $("#camera-model .modal-title").html('');
    $("#camera-model").modal('hide');

    return false;
}
