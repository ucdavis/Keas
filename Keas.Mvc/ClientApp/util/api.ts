import 'isomorphic-fetch';

const doFetch = async (
  url: string,
  antiForgeryToken: string,
  init?: RequestInit
): Promise<any> => {
  const res = await fetch(url, {
    ...init,
    credentials: 'include',
    headers: [
      ['Accept', 'application/json'],
      ['Content-Type', 'application/json'],
      ['RequestVerificationToken', antiForgeryToken]
    ]
  });

  if (!res.ok) {
    const errorText = await res.text();
    throw new Error(errorText);
  }

  return await res.json();
};

const createFetch = (antiForgeryToken: string) => (url, init) =>
  doFetch(url, antiForgeryToken, init);

export { createFetch, doFetch };
