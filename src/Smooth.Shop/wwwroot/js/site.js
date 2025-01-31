class ProgressBar {
    constructor(file, container) {
        this.file = file; // Store the file reference
        this.container = container; // The parent container to append the progress bar to
        this.progressWrapper = document.createElement("div");
        this.progressWrapper.style.marginBottom = "10px";

        this.fileLabel = document.createElement("span");
        this.fileLabel.textContent = `Uploading: ${file.name}`;

        this.progressBar = document.createElement("progress");
        this.progressBar.max = 100;
        this.progressBar.value = 0;
        this.progressBar.style.width = "100%";
        this.progressBar.style.marginTop = "5px";

        this.progressWrapper.appendChild(this.fileLabel);
        this.progressWrapper.appendChild(this.progressBar);
        this.container.appendChild(this.progressWrapper);
    }

    update(progress) {
        this.progressBar.value = progress;
    }

    complete() {
        this.fileLabel.textContent = `Uploaded: ${this.file.name}`;
        this.progressBar.value = 100;
    }

    error(errorMessage) {
        this.fileLabel.textContent = `Error: ${this.file.name} - ${errorMessage}`;
        this.progressBar.style.backgroundColor = "red";
    }
}


class SasTokenService {
    constructor(endpoint) {
        this.endpoint = endpoint;
    }

    getSasToken(fileName, connectionId, callback) {
        const xhr = new XMLHttpRequest();
        const encodedFileName = encodeURIComponent(fileName);
        const encodedConnectionId = encodeURIComponent(connectionId);
        const url = `${this.endpoint}?fileName=${encodedFileName}&connectionId=${encodedConnectionId}`;

        xhr.open("GET", url, true);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    try {
                        const response = JSON.parse(xhr.responseText);
                        if (callback && typeof callback === "function") {
                            callback(null, response.sasTokenUrl);
                        }
                    } catch (error) {
                        console.error("Error parsing response:", error);
                        if (callback) callback(error, null);
                    }
                } else {
                    console.error(`Failed to get SAS token. Status: ${xhr.status}, Message: ${xhr.statusText}`);
                    if (callback) callback(new Error(xhr.statusText), null);
                }
            }
        };

        xhr.onerror = function () {
            console.error("An error occurred during the request.");
            if (callback) callback(new Error("Network error"), null);
        };

        xhr.send();
    }
}

class FileUploader {
    constructor(sasTokenService) {
        this.sasTokenService = sasTokenService;
    }

    uploadFile(file, connectionId, progressBar, onComplete, onError) {
        this.sasTokenService.getSasToken(file.name, connectionId, (error, sasTokenUrl) => {
            if (error) {
                console.error("Failed to fetch SAS token:", error);
                progressBar.error(error.message);
                if (onError) onError(error);
                return;
            }

            const xhr = new XMLHttpRequest();

            xhr.open("PUT", sasTokenUrl, true);

            xhr.upload.onprogress = function (event) {
                if (event.lengthComputable) {
                    const progress = (event.loaded / event.total) * 100;
                    progressBar.update(progress);
                }
            };

            xhr.onload = function () {
                if (xhr.status === 200 || xhr.status === 201) {
                    console.log(`File uploaded successfully: ${file.name}`);
                    progressBar.complete();
                    if (onComplete) onComplete(xhr.responseText);
                } else {
                    const errorMessage = `Upload failed with status ${xhr.status}`;
                    console.error(errorMessage);
                    progressBar.error(errorMessage);
                    if (onError) onError(new Error(errorMessage));
                }
            };

            xhr.onerror = function () {
                const errorMessage = "Network error during file upload.";
                console.error(errorMessage);
                progressBar.error(errorMessage);
                if (onError) onError(new Error(errorMessage));
            };

            xhr.setRequestHeader("x-ms-blob-type", "BlockBlob");
            xhr.send(file);
        });
    }
}


class SignalRService {
    constructor(hubUrl) {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        this.connectionId = null;
        this.onConnectedCallbacks = [];
    }

    async startConnection() {
        try {
            await this.connection.start();
            this.connectionId = this.connection.connectionId;
            console.log("Connected to SignalR hub");
            console.log("Connection ID:", this.connectionId);

            this.onConnectedCallbacks.forEach(callback => callback(this.connectionId));
        } catch (err) {
            console.error("Error connecting to SignalR:", err);
        }
    }

    getConnectionId() {
        return this.connectionId;
    }

    onConnected(callback) {
        alert('callBack');
        if (this.connectionId) {
            callback(this.connectionId);
        } else {
            this.onConnectedCallbacks.push(callback);
        }
    }
}
