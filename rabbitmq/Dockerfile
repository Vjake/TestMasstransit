FROM rabbitmq:3.7.17-management

ENV RABBITMQ_CONFIG_FILE /etc/rabbitmq/clusterrabbitmq.conf
COPY config.conf /etc/rabbitmq/clusterrabbitmq.conf
COPY definitions.json /etc/rabbitmq/clusterdefinitions.json

RUN chmod 777 /etc/rabbitmq/clusterrabbitmq.conf
RUN chmod 777 /etc/rabbitmq/clusterdefinitions.json
RUN apt update && apt install -y iproute2