import boto3
import json
import concurrent.futures
import traceback

lambda_client = boto3.client('lambda')

input_data = {
    "url": "https://www.google.com"
}

payload = json.dumps(input_data).encode('utf-8')

# Define the number of concurrent invocations
num_invocations = 1000

# Define a function to invoke the Lambda function
def invoke_lambda():
    try:
        response = lambda_client.invoke(FunctionName="cete_poc", Payload=payload, InvocationType='Event', LogType='Tail')
        print(response)
    except Exception as e:
        traceback.print_exc()

# Invoke the Lambda function concurrently using multithreading
with concurrent.futures.ThreadPoolExecutor() as executor:
    futures = [executor.submit(invoke_lambda) for _ in range(num_invocations)]
