
export function handleLogout(userID) {
    let url = "http://localhost:8080/member/guest/logout";

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
export function handleOpenNewStore(userID , storeName) {
    let url = "http://localhost:8080/api/guest/open-store";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            storeName: storeName,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleWriteItemReview(userID, storeID, itemID, review) {
    let url = "http://localhost:8080/api/member/write-item-review";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID: userID,
            storeID: storeID,
            itemID:itemID,
            review:review
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleGetItemReviews(storeID, itemID) {
    let url = "http://localhost:8080/api/member/item-reviews";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            storeID: storeID,
            itemID:itemID,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleAddItemStore(userID , storeID, itemName , itemCategory , itemPrice , quantity) {
    let url = "http://localhost:8080/api/member/add-item-store";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID:userID,
            storeID: storeID,
            itemID:itemName,
            itemCategory:itemCategory,
            itemPrice:itemPrice,
            quantity:quantity,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleRemoveItemStore(userID,storeID, itemID) {
    let url = "http://localhost:8080/api/member/rm-item-store";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID:userID,
            storeID: storeID,
            itemID:itemID,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleEditItemStore(userID , storeID, itemName , itemCategory , itemPrice , quantity) {
    let url = "http://localhost:8080/api/member/edit-item-store";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID:userID,
            storeID: storeID,
            itemID:itemName,
            itemCategory:itemCategory,
            itemPrice:itemPrice,
            quantity:quantity,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}

export function handleAppointStoreManager(userID, storeID, userEmail) {
    let url = "http://localhost:8080/api/member/appoint-store-manager";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID:userID,
            storeID: storeID,
            userEmail: userEmail
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}
export function handleAppointStoreManagerPremission(userID, storeID, userEmail , premissions) {
    let url = "http://localhost:8080/api/member/appoint-store-manager-per";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userID:userID,
            storeID: storeID,
            userEmail: userEmail,
            premissions:premissions
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}