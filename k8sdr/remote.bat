winscp.com /command "open sftp://root@home.kube" "put bin\Debug /root/" "exit" /privatekey=Z:\Dokument\SSH\fredrik.ppk
ssh -i Z:\Dokument\SSH\fredrik root@home.kube mono /root/Debug/k8sdr.exe