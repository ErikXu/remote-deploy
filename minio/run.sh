mkdir -p /opt/minio/data

docker rm -f minio

docker run -d -p 9000:9000 --name minio \
-e "MINIO_ACCESS_KEY={access_key}" \
-e "MINIO_SECRET_KEY={secret_key}" \
-v /opt/minio/data:/data \
minio/minio server /data
