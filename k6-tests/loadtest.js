import http from 'k6/http';
import { sleep, check } from 'k6';
import { Rate } from 'k6/metrics';

// Define custom metrics (error rate in this case)
export let errorRate = new Rate('errors');

// Define load testing configuration
export let options = {
    stages: [
        { duration: '30s', target: 10 },  // Ramp up to 10 virtual users over 30 seconds
        { duration: '1m', target: 50 },   // Stay at 50 virtual users for 1 minute
        { duration: '10s', target: 0 },   // Ramp down to 0 users over 10 seconds
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests must complete below 500ms
        errors: ['rate<0.01'],            // Error rate should be below 1%
    },
};

// Load test behavior
export default function () {
    let res = http.get('https://localhost:5001/Search?query=test'); // Change to your app's endpoint

    // Check response and track errors
    check(res, {
        'status is 200': (r) => r.status === 200,
    });

    errorRate.add(res.status !== 200);

    sleep(1); // Simulate user think time between requests
}

//Running in the cloud:
//If you want to run larger distributed tests, you can use k6 Cloud.Just replace the run command with:
let payload = JSON.stringify({ name: 'test', value: 'data' });
let params = { headers: { 'Content-Type': 'application/json' } };

let res = http.post('https://localhost:5001/api/data', payload, params);

//Custom Metrics:
//You can add custom metrics to track specific aspects of your application:
import { Counter } from 'k6/metrics';

let myCounter = new Counter('my_custom_metric');

export default function () {
    // Increment the counter every request
    myCounter.add(1);
}
