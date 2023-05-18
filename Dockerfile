FROM amazon/aws-lambda-dotnet:6

# Install necessary packages
RUN yum update -y && yum install wget -y

# Set up working directory
WORKDIR ${LAMBDA_TASK_ROOT}

# Copy the function code to the container
COPY ./bin/Release/net6.0/linux-x64/publish .

# Set the command to execute
CMD ["cete_poc::cete_poc.Function::FunctionHandler"]
