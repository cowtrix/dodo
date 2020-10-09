import { api } from './api-service';

function spyOnFetch(contentType = 'application/json', ok = true, responseText = '{"test": 123}') {
    const headers = new Map();
    headers.set('content-type', contentType);

    jest.spyOn(global, 'fetch')
    .mockImplementation(() => Promise.resolve({
        headers,
        ok,
        text: () => Promise.resolve(responseText)
    }));
}


describe('api-service tests', () => {
    test('It should return parsed data upon receiving a successful response with the content-type of application/json', () => {
        spyOnFetch('application/json', true, '{"test": 123}');
        expect(api('http://test.com', 'get', {})).resolves.toEqual({
            test: 123
        })
    })

    test('It should return text upon receiving a successful response with the content-type other than application/json', () => {
        spyOnFetch('text/plain', true, '{"test": 123}');
        expect(api('http://test.com', 'get', {})).resolves.toEqual('{"test": 123}')
    })

    test('It should reject upon receiving an unsuccessful response', () => {
        spyOnFetch('text/plain', false, '{"test": 123}');
        expect(api('http://test.com', 'get', {})).rejects.toBeDefined();
    })
})