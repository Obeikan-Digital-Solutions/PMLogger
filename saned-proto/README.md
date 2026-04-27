# saned-proto

proto files for all saned services

## How to generate code

1. Install protoc
2. Install protoc-gen-php
3. Run `protoc --php_out=../saned-php/src/ --proto_path=./ *.proto`

## How to generate docs

1. Install protoc
2. Install protoc-gen-doc
3. Run `protoc --doc_out=../saned-docs/ --doc_opt=html,index.html --proto_path=./ *.proto`

## Add this to your project

1. First make sure you added saned-proto to gitignore in your project repo
   Run `echo "saned-proto" >> .gitignore`
2. Clone this repo  
   Run `git clone https://github.com/KitSys/saned-proto.git`
3. make sure you have the latest version  
   Run `git pull`
4. add your proto files to the saned-proto folder
5. then push your changes to the saned-proto repo
6. you must follow versioning rules for saned-proto folder

- `saned-proto/api/your-service-name/v1/your-service-name.proto`

## how to add a new service

we will use `user` service as an example

1. create a new folder `saned-proto/api/user/v1`
2. create a new file `saned-proto/api/user/v1/user.proto`
    - add the following code to the user.proto file 
   ```proto
   syntax = "proto3";
    
   package api.cinema.v1;
   import "api/user/v1/message.proto";
    option php_namespace = "Saned\\Shared\\Services\\User\\v1";
    option php_metadata_namespace = "Saned\\Shared\\Services\\User\\v1\\GPBMetadata";
   
    service User {
         rpc GetUser (api.user.v1.dto.GetUserRequest) returns (api.user.v1.dto.GetUserResponse) {}
    }
     ```
3. create a new file `saned-proto/api/user/v1/message`
    - add the following code to the message.proto file
    - this file will contain all the messages or DTO that will be used in the service
    ```proto
   syntax = "proto3";

    package api.user.v1.dto;
   
    option php_namespace = "Saned\\Shared\\Services\\User\\v1\\DTO";
    option php_metadata_namespace = "Saned\\Shared\\Services\\User\\v1\\GPBMetadata";

    message GetUserRequest {
        string id = 1;
    }
    message GetUserResponse {
        string id = 1;
        string name = 2;
    }
    ```
