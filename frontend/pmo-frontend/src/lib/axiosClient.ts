import axios from "axios";

export const api = axios.create({
  baseURL: "https://localhost:7177/api",
  // Treat 400 as a normal response so the { isSuccessful, message } envelope can be read.
  validateStatus: (status) => (status >= 200 && status < 300) || status === 400,
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);