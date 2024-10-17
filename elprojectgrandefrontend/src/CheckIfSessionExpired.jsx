import {useEffect} from "react";
import {toast} from "react-hot-toast";
import {useCookies} from "react-cookie";

export const CheckIfSessionExpired = (setUserLoginCookies) => {
    const [cookies] = useCookies(['user']);
    useEffect(() => {
        if (cookies.user === null) {
            return;
        }
        const pingAuth = async () => {
            const res = await fetch("/api/ping", {
                headers: {
                    "Authorization": "Bearer " + cookies.user
                }
            });
            if (res.status !== 200) {
                toast.error("Your session has expired");
                setUserLoginCookies(null);
            }
        }
        pingAuth();
    }, [])
}