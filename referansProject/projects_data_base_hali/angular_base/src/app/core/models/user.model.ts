export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  avatarUrl?: string;
  role: string;
}
