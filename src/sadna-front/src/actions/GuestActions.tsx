
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
export function handleExit(userID) {
    let url = "http://localhost:8080/api/guest/exit";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
        })
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

export function handleRegister(userID, email , firstName , lastName, password) {
    let url = "http://localhost:8080/api/guest/register";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
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

export function handleLogin(userID, email , password) {
    let url = "http://localhost:8080/api/guest/login";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
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
export function handleStoreInfo(storeID) {
    let url = "http://localhost:8080/api/guest/store-info";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            storeID: storeID,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}

export function handleSearchItems(userID,keyWord="", minPrice=0, maxPrice=-1, ratingItem=-1, category="",ratingStore=-1) {
    let url = "http://localhost:8080/api/guest/search-items";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            keyWord : keyWord,
            minPrice:minPrice,
            maxPrice:maxPrice,
            ratingItem:ratingItem,
            category : category,
            ratingStore:ratingStore,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}
export function handleAddItemCart(userID,storeID , itemID , itemAmount=1) {
    let url = "http://localhost:8080/api/guest/add-item-cart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            storeID: storeID,
            itemID: itemID,
            itemAmount : itemAmount
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleRemoveItemCart(userID,storeID , itemID) {
    let url = "http://localhost:8080/api/guest/rm-item-cart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            storeID: storeID,
            itemID: itemID,
            itemAmount : 0,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleEditItemCart(userID,storeID , itemID , itemAmount) {
    let url = "http://localhost:8080/api/guest/edit-item-cart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            storeID: storeID,
            itemID: itemID,
            itemAmount : itemAmount
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleGetDetailsOnCart(userID) {
    let url = "http://localhost:8080/api/guest/shopping-cart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}
export function handlePurchaseCart(userID, paymentDetails, usersDetails ) {
    let url = "http://localhost:8080/api/guest/purchase-cart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            paymentDetails: paymentDetails,
            usersDetails:usersDetails,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}


export function handleIsAdmin(userID) {
    let url = "http://localhost:8080/api/guest/is-admin";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}

export function handlePlaceBid(userID, itemID, price) {
    let url = "http://localhost:8080/api/guest/place-bid";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            itemID: itemID,
            price: price,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleHandshake() {
    let url = "http://localhost:8080/api/guest/handshake";
  
    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
