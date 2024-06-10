// use front face camera
let useFrontCamera = true;

// current video stream
let videoStream;


// play
function PlayCam() {
    video.play();
    btnPlay.classList.add("d-none");
    btnPause.classList.remove("d-none");
}

// pause
function PauseCam() {
    video.pause();
    btnPause.classList.add("d-none");
    btnPlay.classList.remove("d-none");
}

function getFrame(dotNetHelper) {
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    canvas.getContext('2d').drawImage(video, 0, 0);
    let dataUrl = canvas.toDataURL("image/png");
    //Invoke ProcessImage Function and send DataUrl as a parameter to it 
    dotNetHelper.invokeMethodAsync('ProcessImage', dataUrl);
}

function SwitchCam() {
    // switch camera
    useFrontCamera = !useFrontCamera;
    initializeCamera();
}


// stop video stream
function stopVideoStream() {
    if (videoStream) {
        videoStream.getTracks().forEach((track) => {
            track.stop();
        });
    }
}

// initialize
async function initializeCamera() {
    const btnPlay = document.querySelector("#btnPlay");
    const btnPause = document.querySelector("#btnPause");
    const btnCapture = document.querySelector("#btnCapture");
    const btnChangeCamera = document.querySelector("#btnChangeCamera");
    const canvas = document.querySelector("#canvas");
    const devicesSelect = document.querySelector("#devicesSelect");
    const video = document.querySelector("#video");
    if (
        !"mediaDevices" in navigator ||
        !"getUserMedia" in navigator.mediaDevices
    ) {
        alert("Camera API is not available in your browser");
        return;
    }

    // video constraints
    const constraints = {
        video: {
            width: {
                min: 180,
                //ideal: 800,
                //max: 2560,
            },
            height: {
                min: 120,
                //ideal: 600,
                //max: 1440,
            },
        },
    };

    stopVideoStream();
    constraints.video.facingMode = useFrontCamera ? "user" : "environment";

    try {
        videoStream = await navigator.mediaDevices.getUserMedia(constraints);
        video.srcObject = videoStream;
    } catch (err) {
        alert("Could not access the camera" + err);
    }
}