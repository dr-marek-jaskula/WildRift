import SearchBar from "./SearchBar";
import styles from "../../styles/Header.module.scss";
import { useState } from "react";
import MobileMenu from "./MobileMenu";
import Link from "next/link";
import { IconButton } from "@mui/material";

export default function Header() {
	const [menuItems] = useState(["Champions", "Items", "Runes", "News", "Tier Lists"]);
	const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

	const handleMenu = () => {
		setMobileMenuOpen(!mobileMenuOpen);
	};


	return (
		<nav className={styles.navbar}>
			<IconButton sx={{ m: 1 }} onClick={handleMenu}>
				<svg width="40" height="30" viewBox="0 0 40 30" fill="none" xmlns="http://www.w3.org/2000/svg">
					<rect y="24" width="40" height="6" fill="#B08C8C"/>
					<rect y="12" width="40" height="6" fill="#B08C8C"/>
					<rect width="40" height="6" fill="#B08C8C"/>
				</svg>
			</IconButton>
			{/* <button>Dark Mode Toggle</button>
			<SearchBar /> */}
			<ul className={styles.headerMenu}>
				{menuItems.map(item => 
					<li className={styles.menuItem} key={item}>
						<Link href={`/${item.toLowerCase()}`}>{item}</Link>
					</li>
				)}
			</ul>
			<MobileMenu handleMenu={handleMenu} menuOpen={mobileMenuOpen} menuItems={menuItems} />
		</nav>
	);
}