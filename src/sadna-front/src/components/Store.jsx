import React from 'react';

export function Store({ store }) {
  return (
    <div>
      <h2>{store.name}</h2>
      <button>Edit Store</button>
    </div>
  );
}
