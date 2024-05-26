class BackendService {
    static url = "http://localhost:5000/api/v1"

    static async getAllRates() {
        try {
            const response = await fetch(`${this.url}/exchange/allRates`);
            return await response.json();
        } catch (error) {
            console.error(error);
            return null;
        }
    }

    static async GetCurrencyRateHistory(currency, period) {
        try {
            const response = await
                fetch(`${this.url}/exchange/currencypricehistory/${currency}?period=${period}`);
            return await response.json();
        } catch (error) {
            console.error(error);
            return null;
        }
    }

    static async Register(email, password) {
        try {
            const response = await
                fetch(`${this.url}/auth/register`, {
                    method: "POST",
                    body: JSON.stringify({
                        email,
                        password
                    }),
                    headers: new Headers({'content-type': 'application/json'})
                });
            return await response.json();
        } catch (error) {
            return error;
        }
    }

    static async Login(email, password) {
        try {
            const response = await
                fetch(`${this.url}/auth/login`, {
                    method: "POST",
                    body: JSON.stringify({
                        email,
                        password
                    }),
                    headers: new Headers({'content-type': 'application/json'})
                });
            return await response.json();
        } catch (error) {
            return error;
        }
    }

    static async CurrencySubscribe(currencyCode, time, token) {
        try {
            const response = await fetch(`${this.url}/subscription/subscribe?currencyCode=${currencyCode}&time=${time}`, {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });
            return await response.json();
        } catch (error) {
            return error;
        }
    }
    static async CurrencyUnsubscribe(currencyCode, token) {
        try {
            const response = await fetch(`${this.url}/subscription/unsubscribe?currencyCode=${currencyCode}`, {
                method: "GET",
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });
            return await response.json();
        } catch (error) {
            return error;
        }
    }
}

export default BackendService;