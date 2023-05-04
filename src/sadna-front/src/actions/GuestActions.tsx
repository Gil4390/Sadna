
export function handleEnter() {
    let url = "http://localhost:8080/api/guest/enter";

    return fetch(url, {
        method: 'GET',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}

export function handleIsSystemInit() {
    let url = "http://localhost:8080/api/admin/is-system-init";

    return fetch(url, {
        method: 'GET',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}

export function handleRegister(userId, email , firstName , lastName, password) {
    let url = "http://localhost:8080/api/guest/register";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: userId,
            email: email,
            firstName: firstName,
            lastName: lastName,
            password :password,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}

export function handleLogin(userId, email , password) {
    let url = "http://localhost:8080/api/guest/login";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: userId,
            email: email,
            password :password,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}