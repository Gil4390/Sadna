
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
export function handleItemByName(userID,itemName, minPrice, maxPrice, ratingItem, category, ratingStore) {
    let url = "http://localhost:8080/api/guest/item-by-name";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            minPrice:minPrice,
            maxPrice:maxPrice,
            ratingItem:ratingItem,
            category : category,
            ratingStore:ratingStore,
            itemName:itemName
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleItemByCategory(userID,category, minPrice, maxPrice, ratingItem, ratingStore) {
    let url = "http://localhost:8080/api/guest/item-by-category";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
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
        return Promise.resolve(data)
    })
}
export function handleItemByKeyWord(userID,keyWord, minPrice=0, maxPrice=Number.MAX_VALUE, ratingItem=-1, ratingStore=-1) {
    let url = "http://localhost:8080/api/guest/item-by-keys-word";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            minPrice:minPrice,
            maxPrice:maxPrice,
            ratingItem:ratingItem,
            keyWord : keyWord,
            ratingStore:ratingStore,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleAddItemCart(userID,storeID , itemID , itemAmount) {
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
export function handleRemoveItemCart(userID,storeID , itemID , itemAmount) {
    let url = "http://localhost:8080/api/guest/rm-item-cart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            storeID: storeID,
            itemID: itemID,
            itemAmount : null
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
export function handleGetShoppingCart(userID) {
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
        return Promise.resolve(data)
    })
}
export function handlePurchaseCart(userID) {
    let url = "http://localhost:8080/api/guest/purchase-cart";

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