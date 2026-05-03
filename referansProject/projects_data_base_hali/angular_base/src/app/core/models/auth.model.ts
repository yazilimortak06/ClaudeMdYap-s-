export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}

export interface TokenPayload {
  sub: string;
  email: string;
  role: string | string[];
  exp: number;
  iat: number;
  jti: string;
}
