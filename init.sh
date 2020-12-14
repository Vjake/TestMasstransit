#!/bin/bash

if [[ "${#@}" == "0" ]]
then
    echo "./init.sh [address of docker host]"
    exit 1
fi

if [[ "${#@}" == "2" ]] && [[ "$2" == "10" ]]
then
    function docker(){
        docker.exe "$@"
    }
    function docker-compose(){
        docker-compose.exe "$@"
    }
fi

DOCKER_ADDRESS=$1
RABBITMQ1_UI=${DOCKER_ADDRESS}:15673
RABBITMQ2_UI=${DOCKER_ADDRESS}:15674
RABBITMQ3_UI=${DOCKER_ADDRESS}:15675

function get_rabbitmq_server_pid(){
    docker exec $1 /bin/bash -c "ps -ax | grep rabbitmq-server | head -n 1 | awk '{ print \$1 }'"
}

function wait_rabbitmq(){
    echo Waiting for rabbitmq...
    while ! curl $RABBITMQ1_UI &> /dev/null || ! curl $RABBITMQ2_UI &> /dev/null || ! curl $RABBITMQ3_UI &> /dev/null
    do
        sleep 1
    done
    
    echo RabbitMQs are up
}

function cluster_rabbitmq(){
    echo Clustering $1 with $2...
    
    docker exec -it $1 /bin/bash -c "rabbitmqctl stop_app && rabbitmqctl reset && rabbitmqctl join_cluster rabbit@$2 && rabbitmqctl start_app"
    
    echo Clustered
}

docker-compose -p r up -d --force --build
wait_rabbitmq

cluster_rabbitmq rabbitmq2 rabbitmq1
cluster_rabbitmq rabbitmq3 rabbitmq1

echo Everything is set!