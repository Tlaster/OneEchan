//'use strict';
//var OSS = OSS.Wrapper;
//var progress = function progress(p) {
//    return function (done) {
//        var bar = document.getElementById('progress-bar');
//        bar.style.width = Math.floor(p * 100) + '%';
//        bar.innerHTML = Math.floor(p * 100) + '%';
//        done();
//    }
//};

function progress(p) {
    var bar = document.getElementById("progress-bar");
    bar.style.width = p + "%";
    bar.innerHTML = p + "%";
}

var uploader;

window.onload = new function () {
    uploader = new VODUpload({
        // 文件上传失败
        'onUploadFailed': function (uploadInfo, code, message) {
            console.log("onUploadFailed: file:" + uploadInfo.file.name + ",code:" + code + ", message:" + message);
        },
        // 文件上传完成
        'onUploadSucceed': function (uploadInfo) {
            console.log("onUploadSucceed: " + uploadInfo.file.name + ", endpoint:" + uploadInfo.endpoint + ", bucket:" + uploadInfo.bucket + ", object:" + uploadInfo.object);
            $("#FileName").val(uploadInfo.file.name);
            $("#Endpoint").val(uploadInfo.endpoint);
            $("#ObjectName").val(uploadInfo.object);
            $("#Bucket").val(uploadInfo.bucket);
            $("#upload-callback-form").submit();
        },
        // 文件上传进度
        'onUploadProgress': function (uploadInfo, totalSize, uploadedSize) {
            progress(Math.ceil(uploadedSize * 100 / totalSize));
        },
        // STS临时账号会过期，过期时触发函数
        'onUploadTokenExpired': function () {
            console.log("onUploadTokenExpired");
            uploadFile(false);
        },
        // 开始上传
        'onUploadstarted': function (uploadInfo) {
            console.log("onUploadStarted:" + uploadInfo.file.name + ", endpoint:" + uploadInfo.endpoint + ", bucket:" + uploadInfo.bucket + ", object:" + uploadInfo.object);
        }
    });
}
function uploadFile(isInit) {
    $.ajax({
        type: "post",
        dataType: "json",
        headers: {
            "RequestVerificationToken": getToken()
        },
        url: getSignatureUrl(),
        success: function (data) {
            if (isInit) {
                uploader.init(data.accessKeyId, data.accessKeySecret, data.securityToken, data.expiration);
                uploader.addFile(document.getElementById('file').files[0], data.endpoint, data.bucket, random_string(20) + get_suffix($("#file").val()), '');
                uploader.startUpload();
            } else {
                uploader.resumeUploadWithToken(data.accessKeyId, data.accessKeySecret, data.securityToken, data.expiration);
            }
            //var client = new OSS({
            //    region: data.region,
            //    accessKeyId: data.accessKeyId,
            //    accessKeySecret: data.accessKeySecret,
            //    stsToken: data.securityToken,
            //    bucket: data.bucket,
            //    secure: true
            //});
            //client.multipartUpload(random_string(20) + get_suffix($("#file").val()), document.getElementById('file').files[0], {
            //    progress: progress
            //}).then(function (res) {
            //    console.log('upload success: %j', res);
            //});
        },
        error: function (err, scnd) {

        }
    });
}

function random_string(len) {
    len = len || 32;
    var chars = 'ABCDEFGHJKMNPQRSTWXYZabcdefhijkmnprstwxyz2345678';
    var maxPos = chars.length;
    var pwd = '';
    for (var i = 0; i < len; i++) {
        pwd += chars.charAt(Math.floor(Math.random() * maxPos));
    }
    return pwd;
}

function get_suffix(filename) {
    var pos = filename.lastIndexOf('.');
    var suffix = '';
    if (pos != -1) {
        suffix = filename.substring(pos);
    }
    return suffix;
}

$("#upload-file").on('click', function (event) {
    uploadFile(true);
});